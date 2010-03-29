using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using CouchNet.Enums;
using CouchNet.Model;
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

        public readonly string Name;

        public CouchServerResponse ServerResponse { get; private set; }

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

        public IEnumerable<ICouchDocument> GetAll()
        {
            return GetAll(null, null, null, null);
        }

        public IEnumerable<ICouchDocument> GetAll(int? limit, string startkey, string endkey, bool? descending)
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

            var response = _connection.Get(path);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }

                return new ICouchDocument[0];
            }

            var result = JsonConvert.DeserializeObject<CouchViewResult<CouchResultRow<ResultFragment>>>(response.Content, _settings);

            return result.Rows.Select(row => new SimpleCouchDocument { Id = row.Id, Revision = row.Value.Revision }).Cast<ICouchDocument>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return GetAll<T>(null, null, null, null);
        }
        
        public IEnumerable<T> GetAll<T>(int? limit, string startkey, string endkey, bool? descending)
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

            var response = _connection.Get(path);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }

                return new List<T>();
            }

            var result = JsonConvert.DeserializeObject<CouchViewResult<CouchBulkResultRow<T>>>(response.Content, _settings);

            return result.Rows.Select(row => row.Document);
        }

        //public IEnumerable<ICouchDocument> GetAllBySequence()
        //public IEnumerable<ICouchDocument> GetAllBySequence(int? limit, string startkey, string endkey, bool? descending

        //public IEnumerable<T> GetSelected<T>(IEnumerable<string> ids)     

        public void Add(ICouchDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException();
            }

            Save(document, true);
        }

        //void Add(IEnumerable<ICouchDocument> documents, bool AllOrNothing)

        public void Save(ICouchDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException();
            }

            Save(document, false);
        }

        //void Save(IEnumerable<ICouchDocument> documents, bool AllOrNothing)

        public void Delete(string id, string revision)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(revision))
            {
                throw new ArgumentNullException();
            }

            var qs = new QueryString().Add("rev", revision);

            var path = string.Format("{0}/{1}{2}", Name, id, qs);
            var response = _connection.Delete(path);

            ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
        }

        public void Delete(ICouchDocument document)
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
            var response = _connection.Delete(path);

            ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
        }

        //public void Copy(string fromId, string toId)
        //public void Copy(ICouchDocument from, string toId)
        //public void Copy(string fromId, string toId, string revision)
        //public void Copy(ICouchDocument from, ICouchDocument to)
        
        #endregion

        #region Other

        public int DocumentCount()
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
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
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
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
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
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void Save(ICouchDocument document, bool isNew)
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

            var response = _connection.Put(path, jsonString);

            ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
        }

        private T Get<T>(string id, QueryString queryString) where T : ICouchDocument
        {
            var path = string.Format("{0}/{1}{2}", Name, id, queryString);

            var response = _connection.Get(path);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private int GetRevisionsLimit()
        {
            var path = string.Format("{0}/{1}", Name, "_revs_limit");
            var response = _connection.Get(path);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }

                return -1;
            }

            int limit;

            if (int.TryParse(response.Content, out limit))
            {
                return limit;
            }

            return limit;
        }

        private void SetRevisionsLimit(int limit)
        {
            var path = string.Format("{0}/{1}", Name, "_revs_limit");
            var response = _connection.Put(path, limit.ToString());

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }
            }
        }

        #endregion

    }
}