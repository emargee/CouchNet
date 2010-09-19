using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Linq;

using CouchNet.Enums;
using CouchNet.Exceptions;
using CouchNet.HttpTransport;
using CouchNet.Impl.ResultParsers;
using CouchNet.Impl.ServerResponse;
using CouchNet.Internal;
using CouchNet.Utils;
using Newtonsoft.Json;

//  A database must be named with all lowercase characters (a-z), digits (0-9), or any of the _$()+-/ characters and must end with a slash in the URL 
//
// Ref: http://books.couchdb.org/relax/reference/change-notifications
// Implement: _changes
// Implement: _changes?since=3 (default:0)
// Implement: _changes?feed=normal|longpoll|continuous (default:normal)
// Implement: _changes?heartbeat=60000 (default:60000) milliseconds
// Implement: _changes?timeout=60000 (default:60000) milliseconds
// Implement: _changes?filter=app/important (default:none) - _filter on a view.
// Implement: _changes?include_docs=true (default:false) - include document with result.
//
//=========================================

namespace CouchNet.Impl
{
    public class CouchDatabase : ICouchDatabase
    {
        #region Private Instance Fields

        internal readonly CouchService Service;

        internal IHttpResponse RawResponse { get; set; }

        internal IList<CouchDesignDocument> DesignDocuments;

        #endregion

        public readonly string Name;

        #region Public Properties

        public CouchBulkUpdateBehaviour BulkUpdateBehaviour { get; set; }

        public int RevisionsLimit
        {
            get
            {
                return GetRevisionsLimit();
            }

            set
            {
                SetRevisionsLimit(value);
            }
        }

        #endregion

        #region ctor

        public CouchDatabase(string databaseName, CouchService service)
        {
            if (service == null || string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException();
            }

            Service = service;

            databaseName = databaseName.ToLower();

            if (databaseName.Contains("/"))
            {
                databaseName = databaseName.Replace("/", "%2F"); //Should encode whole name ? 
            }

            Name = databaseName;

            BulkUpdateBehaviour = CouchBulkUpdateBehaviour.NonAtomic;

            DesignDocuments = new List<CouchDesignDocument>();
        }

        #endregion

        #region Basic Operation Methods

        public T Get<T>(string id) where T : ICouchDocument
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            return Get<T>(id, new QueryString());
        }

        public T Get<T>(string id, string revision) where T : ICouchDocument
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(revision))
            {
                throw new ArgumentNullException();
            }

            var qs = new QueryString();

            if (!string.IsNullOrEmpty(revision))
            {
                qs.Add("rev", revision);
            }

            return Get<T>(id, qs);
        }

        public T Get<T>(string id, CouchDocumentOptions options) where T : ICouchDocument
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            var qs = new QueryString();

            if (options.IsOptionSelected(CouchDocumentOptions.RevisionInfo))
            {
                qs.Add("revs_info", "true");
            }

            if (options.IsOptionSelected(CouchDocumentOptions.IncludeRevisions))
            {
                qs.Add("revs", "true");
            }

            return Get<T>(id, qs);
        }

        public ICouchQueryResults<T> GetMany<T>(IEnumerable<string> ids) where T : ICouchDocument
        {
            var query = new CouchViewQuery { IncludeDocs = true };

            var path = string.Format("{0}/_all_docs{1}", Name, query);

            RawResponse = Service.Connection.Post(path, JsonConvert.SerializeObject(new { keys = ids }));

            var results = new CouchQueryAllDocumentsResultsParser<T>().Parse(RawResponse);

            return results;
        }

        public ICouchQueryResults<CouchDocument> GetMany(IEnumerable<string> ids)
        {
            var path = string.Format("{0}/_all_docs", Name);
            RawResponse = Service.Connection.Post(path, JsonConvert.SerializeObject(new { keys = ids }));

            var results = new CouchQueryGeneralResultsParser().Parse(RawResponse);

            return results;
        }

        public ICouchQueryResults<CouchDocument> GetAll()
        {
            return GetAll(new CouchViewQuery());
        }

        public ICouchQueryResults<CouchDocument> GetAll(CouchViewQuery query)
        {
            var path = string.Format("{0}/_all_docs", Name);

            if (!string.IsNullOrEmpty(query.ToString()))
            {
                path = path + query;
            }

            RawResponse = Service.Connection.Get(path);

            var results = new CouchQueryGeneralResultsParser().Parse(RawResponse);

            return results;
        }

        public ICouchQueryResults<T> GetAll<T>() where T : ICouchDocument
        {
            return GetAll<T>(new CouchViewQuery());
        }

        public ICouchQueryResults<T> GetAll<T>(CouchViewQuery query) where T : ICouchDocument
        {
            query.IncludeDocs = true;

            var path = string.Format("{0}/_all_docs{1}", Name, query);

            RawResponse = Service.Connection.Get(path);

            var results = new CouchQueryAllDocumentsResultsParser<T>().Parse(RawResponse);

            return results;
        }

        public ICouchServerResponse Add(ICouchDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            return Save(document, true);
        }

        public ICouchQueryResults<ICouchServerResponse> AddMany(IEnumerable<ICouchDocument> documents)
        {
            var path = string.Format("{0}/_bulk_docs", Name);
            var bulk = new CouchDocumentCollectionDefinition<ICouchDocument>(documents);

            if (BulkUpdateBehaviour == CouchBulkUpdateBehaviour.AllOrNothing)
            {
                bulk.AllOrNothing = true;
            }

            RawResponse = Service.Connection.Post(path, JsonConvert.SerializeObject(bulk, Formatting.None, CouchService.JsonSettings));

            var results = new CouchQueryBulkOperationResultsParser().Parse(RawResponse);

            return results;
        }

        public ICouchServerResponse Save(ICouchDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException();
            }

            var returnDoc = Save(document, false);

            return returnDoc;
        }

        public ICouchQueryResults<ICouchServerResponse> SaveMany(IEnumerable<ICouchDocument> documents)
        {
            if (documents.Any(doc => string.IsNullOrEmpty(doc.Revision)))
            {
                throw new InvalidOperationException("Updating an existing document requires a 'revision'(_rev) value.");
            }

            var path = string.Format("{0}/_bulk_docs", Name);
            var bulk = new CouchDocumentCollectionDefinition<ICouchDocument>(documents);

            if (BulkUpdateBehaviour == CouchBulkUpdateBehaviour.AllOrNothing)
            {
                bulk.AllOrNothing = true;
            }

            RawResponse = Service.Connection.Post(path, JsonConvert.SerializeObject(bulk, Formatting.None, CouchService.JsonSettings));

            var results = new CouchQueryBulkOperationResultsParser().Parse(RawResponse);

            return results;
        }

        public ICouchServerResponse Delete(string id, string revision)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(revision))
            {
                throw new ArgumentNullException();
            }

            var qs = new QueryString().Add("rev", revision);

            var path = string.Format("{0}/{1}{2}", Name, id, qs);
            RawResponse = Service.Connection.Delete(path);

            return new CouchServerResponse(RawResponse);
        }

        public ICouchServerResponse Delete(ICouchDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrEmpty(document.Revision))
            {
                throw new InvalidOperationException("Deleting an existing document requires a 'revision'(_rev) value.");
            }

            var qs = new QueryString().Add("rev", document.Revision);

            var path = string.Format("{0}/{1}{2}", Name, document.Id, qs);
            RawResponse = Service.Connection.Delete(path);

            return new CouchServerResponse(RawResponse);
        }

        public ICouchServerResponse Copy(ICouchDocument from, string toId)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }

            return Copy(from.Id, toId, null);
        }

        public ICouchServerResponse Copy(string fromId, string toId)
        {
            return Copy(fromId, toId, null);
        }

        public ICouchServerResponse Copy(string fromId, string toId, string revision)
        {
            if (string.IsNullOrEmpty(fromId) || string.IsNullOrEmpty(toId))
            {
                throw new ArgumentNullException();
            }

            var path = string.Format("{0}/{1}", Name, fromId);

            if (!string.IsNullOrEmpty(revision))
            {
                RawResponse = Service.Connection.Copy(path, toId + new QueryString().Add("rev", revision));
            }
            else
            {
                RawResponse = Service.Connection.Copy(path, toId);
            }

            var status = new CouchServerResponse(RawResponse);

            return status;
        }

        public bool Exists(ICouchDocument document)
        {
            if(document == null)
            {
                return false;
            }

            return !string.IsNullOrEmpty(document.Id) && Exists(document.Id);
        }

        public bool Exists(string id)
        {
            var path = string.Format("{0}/{1}", Name, id);
            return Service.Connection.Head(path).StatusCode == HttpStatusCode.OK || Service.Connection.Head(path).StatusCode == HttpStatusCode.NotModified;
        }

        #endregion

        #region Design Document

        public CouchDesignDocument CreateDesignDocument(string name)
        {
            var doc = new CouchDesignDocument(name, this);
            DesignDocuments.Add(doc);
            return doc;
        }

        public CouchDesignDocument DesignDocument(string name)
        {
            var documentName = "_design/" + name;

            var result = Get<CouchDesignDocumentDefinition>(documentName);

            if (result == null)
            {
                throw new CouchDocumentNotFoundException(name);
            }

            return new CouchDesignDocument(result, this);
        }

        #endregion

        #region Temp View

        public ICouchQueryResults<T> ExecuteTempView<T>(CouchTempView view, CouchViewQuery query) where T : ICouchDocument
        {
            if (string.IsNullOrEmpty(view.Map) && string.IsNullOrEmpty(view.Reduce))
            {
                throw new ArgumentException("Please provide MAP or MAP & REDUCE functions.");
            }

            if (string.IsNullOrEmpty(view.Map) && !string.IsNullOrEmpty(view.Reduce))
            {
                throw new ArgumentException("Cannot have a REDUCE function without a MAP function.");
            }

            var path = string.Format("{0}/{1}{2}", Name, view, query);
            RawResponse = Service.Connection.Post(path, view.ToJson(), "application/json");

            var results = new CouchQueryViewResultsParser<T>().Parse(RawResponse);

            return results;
        }

        #endregion

        #region Other

        public int DocumentCount()
        {
            var status = Status();
            return status.IsOk ? Status().DocumentCount : -1;
        }

        #endregion

        #region Information Objects

        public CouchDatabaseStatusResponse Status()
        {
            var path = Name;
            RawResponse = Service.Connection.Get(path);

            return new CouchDatabaseStatusResponse(RawResponse, CouchService.JsonSettings);
        }

        #endregion

        #region Private Methods

        private ICouchServerResponse Save(ICouchDocument document, bool isNew)
        {
            if (!isNew && string.IsNullOrEmpty(document.Revision))
            {
                throw new InvalidOperationException("Updating an existing document requires a 'revision'(_rev) value.");
            }

            if (string.IsNullOrEmpty(document.Id))
            {
                document.Id = Guid.NewGuid().ToString().ToLower().Replace("{", string.Empty).Replace("}", string.Empty).Replace("-", string.Empty);
            }

            string jsonString;

            try
            {
                jsonString = JsonConvert.SerializeObject(document, Formatting.None, CouchService.JsonSettings);
            }

            catch (JsonSerializationException ex)
            {
                throw new InvalidCastException("Unable to covert object to JSON.", ex);
            }

            var path = string.Format("{0}/{1}", Name, document.Id);

            RawResponse = Service.Connection.Put(path, jsonString);

            return new CouchServerResponse(RawResponse);
        }

        private T Get<T>(string id, QueryString queryString) where T : ICouchDocument
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(id);
            }

            if(queryString == null)
            {
                queryString = new QueryString();
            }

            if (Service.EnableValidation)
            {
                if(!Exists(id))
                {
                    throw new CouchDocumentNotFoundException(id);
                }
            }

            var path = string.Format("{0}/{1}{2}", Name, id, queryString);

            RawResponse = Service.Connection.Get(path);

            if (RawResponse.StatusCode != HttpStatusCode.OK && RawResponse.StatusCode != HttpStatusCode.NotModified)
            {
                throw new CouchDocumentNotFoundException(id);
            }

            return JsonConvert.DeserializeObject<T>(RawResponse.Data);
        }

        private int GetRevisionsLimit()
        {
            var path = string.Format("{0}/{1}", Name, "_revs_limit");
            RawResponse = Service.Connection.Get(path);

            if (RawResponse.StatusCode != HttpStatusCode.OK && RawResponse.StatusCode != HttpStatusCode.NotModified)
            {
                return -1;
            }

            int limit;

            if (int.TryParse(RawResponse.Data, out limit))
            {
                return limit;
            }

            return limit;
        }

        private void SetRevisionsLimit(int limit)
        {
            var path = string.Format("{0}/{1}", Name, "_revs_limit");
            RawResponse = Service.Connection.Put(path, limit.ToString());
        }

        #endregion

    }
}