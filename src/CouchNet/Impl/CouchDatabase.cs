using System;
using System.Net;
using CouchNet.Enums;
using CouchNet.Model;
using CouchNet.Utils;
using Newtonsoft.Json;

namespace CouchNet.Impl
{
    public class CouchDatabase<T> where T : class
    {
        private readonly ICouchConnection _connection;

        public readonly string Name;

        public CouchServerResponse ServerResponse { get; private set; }

        private CouchDocumentOptions DocumentOptions { get; set; }

        public CouchDatabase(ICouchConnection connection, string databaseName)
        {
            if(connection == null || string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException();
            }

            _connection = connection;
            Name = databaseName;
        }

        public T Get(string id)
        {
            return Get(id, new QueryString());
        }

        public T Get(string id, string revision)
        {
            var qs = new QueryString();

            if (!string.IsNullOrEmpty(revision))
            {
                qs.Add("rev", revision);
            }

            return Get(id, qs);
        }

        public T Get(string id, CouchDocumentOptions options)
        {
            var qs = new QueryString();

            DocumentOptions = options;          
    
            if(IsOptionSelected(CouchDocumentOptions.RevisionInfo))
            {
                qs.Add("revs_info", "true");
            }

            if (IsOptionSelected(CouchDocumentOptions.IncludeRevisions))
            {
                qs.Add("revs", "true");
            }

            return Get(id, qs);
        }

        private T Get(string id, QueryString queryString)
        {
            var path = string.Format("{0}/{1}{2}",Name, id, queryString);
            var response = _connection.Get(path);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content.Contains("\"error\""))
                {
                    ServerResponse = JsonConvert.DeserializeObject<CouchServerResponse>(response.Content);
                }
                return null;
            }

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private bool IsOptionSelected(CouchDocumentOptions options)
        {
            return (options & DocumentOptions) != CouchDocumentOptions.None;
        }
    }
}