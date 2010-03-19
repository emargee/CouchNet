using System;
using System.Net;
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
    public class CouchDatabase<T> : ICouchDatabase<T> where T : CouchDocument
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

        #region CRUD methods

        public T Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            return Get(id, new QueryString());
        }

        public T Get(string id, string revision)
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

            return Get(id, qs);
        }

        public T Get(string id, CouchDocumentOptions options)
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

            return Get(id, qs);
        }

        public CouchServerResponse Add(T document)
        {
            if (document == null)
            {
                throw new ArgumentNullException();
            }

            return Save(document, true);
        }

        public CouchServerResponse Save(T document)
        {
            if (document == null)
            {
                throw new ArgumentNullException();
            }

            return Save(document, false);
        }

        public CouchServerResponse Delete(string id, string revision)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(revision))
            {
                throw new ArgumentNullException();
            }

            var qs = new QueryString().Add("rev", revision);

            var path = string.Format("{0}/{1}{2}", Name, id, qs);
            var response = _connection.Delete(path);

            ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);

            return ServerResponse;
        }

        public CouchServerResponse Delete(T document)
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

            return ServerResponse;
        }

        #endregion

        #region Database Utilities

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

            if(response.StatusCode != HttpStatusCode.Accepted)
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

        private CouchServerResponse Save(T document, bool isNew)
        {
            if (document == null)
            {
                throw new ArgumentNullException();
            }

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

            return ServerResponse;
        }

        private T Get(string id, QueryString queryString)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            if (queryString == null)
            {
                queryString = new QueryString();
            }

            var path = string.Format("{0}/{1}{2}", Name, id, queryString);
            var response = _connection.Get(path);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }
                return null;
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