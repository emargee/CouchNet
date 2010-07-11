using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using CouchNet.Enums;
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
        private readonly ICouchConnection _connection;

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        internal ICouchResponseMessage RawResponse { get; set; }

        public readonly string Name;

        public CouchServerResponse ErrorResponse { get; private set; }

        public CouchBulkUpdateBehaviour BulkUpdateBehaviour { get; set;}

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

        #region ctor

        public CouchDatabase(ICouchConnection connection, string databaseName)
        {
            if (connection == null || string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException();
            }

            _connection = connection;

            databaseName = databaseName.ToLower();

            if (databaseName.Contains("/"))
            {
                databaseName = databaseName.Replace("/", "%2F"); //Should encode whole name ? 
            }

            Name = databaseName;

            BulkUpdateBehaviour = CouchBulkUpdateBehaviour.NonAtomic;
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

        public IEnumerable<T> GetSelected<T>(IEnumerable<string> ids) where T : ICouchDocument
        {
            var qs = new QueryString { { "include_docs", "true" } };
            var path = string.Format("{0}/_all_docs{1}", Name, qs);

            var keys = new { keys = ids.ToArray() };
            var keySerialized = JsonConvert.SerializeObject(keys);

            RawResponse = _connection.Post(path, keySerialized);

            if (RawResponse.StatusCode != HttpStatusCode.OK)
            {
                if (RawResponse.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
                }

                return new List<T>();
            }

            var result = JsonConvert.DeserializeObject<CouchViewResult<CouchBulkResultRow<T>>>(RawResponse.Content, _settings);

            return result.Rows.SkipWhile(s => s.Value.IsDeleted == true).Select(row => row.Document);
        }

        public IEnumerable<CouchDocument> GetSelected(IEnumerable<string> ids)
        {
            var path = string.Format("{0}/_all_docs", Name);

            var keys = new { keys = ids.ToArray() };
            var keySerialized = JsonConvert.SerializeObject(keys);

            RawResponse = _connection.Post(path, keySerialized);

            if (RawResponse.StatusCode != HttpStatusCode.OK)
            {
                if (RawResponse.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
                }

                return new CouchDocument[0];
            }

            var result = JsonConvert.DeserializeObject<CouchViewResult<CouchResultRow<CouchDocumentSummary>>>(RawResponse.Content, _settings);

            return result.Rows.Select(row => new CouchDocument { Id = row.Id, Revision = row.Value.Revision, Conflicts = row.Value.Conflicts, IsDeleted = row.Value.IsDeleted, DeletedConflicts = row.Value.DeletedConflicts });
        }

        public IEnumerable<CouchDocument> GetAll()
        {
            return GetAll(null, null, null, null);
        }

        public IEnumerable<CouchDocument> GetAll(int? limit, string startkey, string endkey, bool? descending)
        {
            var qs = new QueryString();

            if (limit.HasValue)
            {
                qs.Add("limit", limit.ToString());
            }

            if (!string.IsNullOrEmpty(startkey))
            {
                qs.Add("startkey", "\"" + startkey + "\"");
            }

            if (!string.IsNullOrEmpty(endkey))
            {
                qs.Add("endkey", "\"" + endkey + "\"");
            }

            if (descending.HasValue)
            {
                qs.Add("descending", descending.ToString().ToLower());
            }

            var path = string.Format("{0}/_all_docs", Name);

            if (qs.Count > 0)
            {
                path = path + qs;
            }

            RawResponse = _connection.Get(path);

            if (RawResponse.StatusCode != HttpStatusCode.OK)
            {
                if (RawResponse.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
                }

                return new CouchDocument[0];
            }

            var result = JsonConvert.DeserializeObject<CouchViewResult<CouchResultRow<CouchDocumentSummary>>>(RawResponse.Content, _settings);

            return result.Rows.Select(row => new CouchDocument { Id = row.Id, Revision = row.Value.Revision, Conflicts = row.Value.Conflicts, IsDeleted = row.Value.IsDeleted, DeletedConflicts = row.Value.DeletedConflicts });
        }

        public IEnumerable<T> GetAll<T>() where T : ICouchDocument
        {
            return GetAll<T>(null, null, null, null);
        }
        
        public IEnumerable<T> GetAll<T>(int? limit, string startkey, string endkey, bool? descending) where T : ICouchDocument
        {
            var qs = new QueryString();

            if (limit.HasValue)
            {
                qs.Add("limit", limit.ToString());
            }

            if (!string.IsNullOrEmpty(startkey))
            {
                qs.Add("startkey", "\"" + startkey + "\"");
            }

            if (!string.IsNullOrEmpty(endkey))
            {
                qs.Add("endkey", "\"" + endkey + "\"");
            }

            if (descending.HasValue)
            {
                qs.Add("descending", descending.ToString().ToLower());
            }

            qs.Add("include_docs", "true");

            var path = string.Format("{0}/_all_docs{1}", Name, qs);

            RawResponse = _connection.Get(path);

            if (RawResponse.StatusCode != HttpStatusCode.OK)
            {
                if (RawResponse.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
                }

                return new List<T>();
            }

            var result = JsonConvert.DeserializeObject<CouchViewResult<CouchBulkResultRow<T>>>(RawResponse.Content, _settings);

            return result.Rows.SkipWhile(s => s.Value.IsDeleted == true).Select(row => row.Document);
        }

        public CouchServerResponse Add(ICouchDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            return Save(document, true);
        }

        public IEnumerable<CouchServerResponse> Add(IEnumerable<ICouchDocument> documents)
        {
            var path = string.Format("{0}/_bulk_docs", Name);
            var bulk = new CouchDocumentCollection<ICouchDocument>(documents);

            if(BulkUpdateBehaviour == CouchBulkUpdateBehaviour.AllOrNothing)
            {
                bulk.AllOrNothing = true;
            }

            RawResponse = _connection.Post(path, JsonConvert.SerializeObject(bulk, Formatting.None, _settings));

            if (RawResponse.StatusCode != HttpStatusCode.Created)
            {
                if (RawResponse.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
                }

                return new CouchServerResponse[0];
            }

            return JsonConvert.DeserializeObject<IEnumerable<CouchServerResponse>>(RawResponse.Content, _settings);
        }

        public CouchServerResponse Save(ICouchDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException();
            }

            return Save(document, false);
        }

        public IEnumerable<CouchServerResponse> Save(IEnumerable<ICouchDocument> documents)
        {
            if (documents.Any(doc => string.IsNullOrEmpty(doc.Revision)))
            {
                throw new InvalidOperationException("Updating an existing document requires a 'revision'(_rev) value.");
            }

            var path = string.Format("{0}/_bulk_docs", Name);
            var bulk = new CouchDocumentCollection<ICouchDocument>(documents);

            if (BulkUpdateBehaviour == CouchBulkUpdateBehaviour.AllOrNothing)
            {
                bulk.AllOrNothing = true;
            }

            RawResponse = _connection.Post(path, JsonConvert.SerializeObject(bulk, Formatting.None, _settings));

            if (RawResponse.StatusCode != HttpStatusCode.Created)
            {
                if (RawResponse.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
                }

                return new CouchServerResponse[0];
            }

            return JsonConvert.DeserializeObject<IEnumerable<CouchServerResponse>>(RawResponse.Content, _settings);
        }

        public CouchServerResponse Delete(string id, string revision)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(revision))
            {
                throw new ArgumentNullException();
            }

            var qs = new QueryString().Add("rev", revision);

            var path = string.Format("{0}/{1}{2}", Name, id, qs);
            RawResponse = _connection.Delete(path);

            return JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
        }

        public CouchServerResponse Delete(ICouchDocument document)
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
            RawResponse = _connection.Delete(path);

            return JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
        }

        public CouchServerResponse Copy(ICouchDocument from, string toId)
        {
            return Copy(from.Id, toId, null);
        }

        public CouchServerResponse Copy(string fromId, string toId)
        {
            return Copy(fromId, toId, null);
        }

        public CouchServerResponse Copy(string fromId, string toId, string revision)
        {
            if (string.IsNullOrEmpty(fromId) || string.IsNullOrEmpty(toId))
            {
                throw new ArgumentNullException();
            }

            var path = string.Format("{0}/{1}", Name, fromId);

            if(string.IsNullOrEmpty(revision))
            {
                RawResponse = _connection.Copy(path, toId + new QueryString().Add("rev", revision));    
            }
            else
            {
                RawResponse = _connection.Copy(path, toId);    
            }        

            var status = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);

            if (RawResponse.StatusCode != HttpStatusCode.Created)
            {
                ErrorResponse = status;
            }

            return status;
        }       

        //public void Move(string fromId, string toId)
        //public void Move(ICouchDocument from, string toId)
        
        #endregion

        #region View Methods



        #endregion

        #region Other

        public int Count()
        {
            var status = Status();
            return status != null ? Status().DocumentCount : -1;
        }

        #endregion

        #region Database Utility Methods

        public CouchDatabaseStatus Status()
        {
            var path = Name;
            var response = _connection.Get(path);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }
                return null;
            }

            return JsonConvert.DeserializeObject<CouchDatabaseStatus>(response.Content);
        }

        public bool BeginCompact()
        {
            var path = string.Format("{0}/_compact", Name);
            var response = _connection.Post(path, null);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }
                return false;
            }

            return true;
        }

        public bool BeginViewCleanup()
        {
            var path = string.Format("{0}/_view_cleanup", Name);
            var response = _connection.Post(path, null);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        private CouchServerResponse Save(ICouchDocument document, bool isNew)
        {
            if (!isNew && string.IsNullOrEmpty(document.Revision))
            {
                throw new InvalidOperationException("Updating an existing document requires a 'revision'(_rev) value.");
            }

            if (string.IsNullOrEmpty(document.Id))
            {
                document.Id = Guid.NewGuid().ToString().ToLower().Replace("{", string.Empty).Replace("}", string.Empty).Replace("-", string.Empty);
            }

            var jsonString = string.Empty;

            try
            {
                jsonString = JsonConvert.SerializeObject(document, Formatting.None, _settings);
            }

            catch (JsonSerializationException ex)
            {
                throw new InvalidCastException("Unable to covert object to JSON.", ex);
            }

            var path = string.Format("{0}/{1}", Name, document.Id);

            RawResponse = _connection.Put(path, jsonString);

            var status = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);

            if(RawResponse.StatusCode != HttpStatusCode.Created)
            {
                ErrorResponse = status;    
            }

            return status ;
        }

        private T Get<T>(string id, QueryString queryString) where T : ICouchDocument
        {
            var path = string.Format("{0}/{1}{2}", Name, id, queryString);

            RawResponse = _connection.Get(path);

            if (RawResponse.StatusCode != HttpStatusCode.OK)
            {
                if (RawResponse.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
                }
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(RawResponse.Content);
        }

        private int GetRevisionsLimit()
        {
            var path = string.Format("{0}/{1}", Name, "_revs_limit");
            RawResponse = _connection.Get(path);

            if (RawResponse.StatusCode != HttpStatusCode.OK)
            {
                if (RawResponse.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
                }

                return -1;
            }

            int limit;

            if (int.TryParse(RawResponse.Content, out limit))
            {
                return limit;
            }

            return limit;
        }

        private void SetRevisionsLimit(int limit)
        {
            var path = string.Format("{0}/{1}", Name, "_revs_limit");
            RawResponse = _connection.Put(path, limit.ToString());

            if (RawResponse.StatusCode != HttpStatusCode.Accepted)
            {
                if (RawResponse.Content.Contains("\"error\""))
                {
                    ErrorResponse = JsonConvert.DeserializeObject<CouchServerResponse>(RawResponse.Content);
                }
            }
        }

        #endregion

    }
}