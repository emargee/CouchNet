using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Http;

using CouchNet.Enums;

namespace CouchNet.Impl
{
    public class CouchConnection : ICouchConnection
    {
        internal HttpClient Client;

        #region Private Properties

        internal Uri Address { get; set; }

        internal string Encoding { get; set; }

        #endregion

        #region Public Properties

        public Dictionary<string, string> CustomHeaders { get; set; }

        #endregion

        #region ctor

        public CouchConnection() : this(new UriBuilder("http", "localhost", 5984).Uri) { }

        public CouchConnection(string url) : this(new UriBuilder(FixHost(url)).Uri) { }

        public CouchConnection(string host, int port) : this(new UriBuilder(FixHost(host)) { Port = port }.Uri) { }

        public CouchConnection(string host, int port, string encoding) : this(new UriBuilder(FixHost(host)) { Port = port }.Uri, encoding) { }

        public CouchConnection(Uri uri) : this(uri, "application/json") { }

        public CouchConnection(Uri uri, string encoding)
        {
            Address = uri;
            Encoding = encoding;
            CustomHeaders = new Dictionary<string, string>();
        }

        #endregion

        #region Public Methods

        public string Get(string path)
        {
            return MakeRequest(path, HttpVerb.Get, null, Encoding);
        }

        public string Get(string path, string encoding)
        {
            return MakeRequest(path, HttpVerb.Get, null, encoding);
        }

        public string Put(string path, string data)
        {
            return MakeRequest(path, HttpVerb.Put, data, Encoding);
        }

        public string Put(string path, string data, string encoding)
        {
            return MakeRequest(path, HttpVerb.Put, data, encoding);
        }

        public string Post(string path, string data)
        {
            return MakeRequest(path, HttpVerb.Post, data, Encoding);
        }

        public string Post(string path, string data, string encoding)
        {
            return MakeRequest(path, HttpVerb.Post, data, encoding);
        }

        public string Delete(string path)
        {
            return MakeRequest(path, HttpVerb.Delete, null, Encoding);
        }

        public string Delete(string path, string encoding)
        {
            return MakeRequest(path, HttpVerb.Delete, null, encoding);
        }

        public void DisableCache()
        {
            CustomHeaders.Add("Cache-Control", "no-cache");
        }

        #endregion

        #region Private Methods

        private string MakeRequest(string path, HttpVerb method, string data, string encoding)
        {
            Client = new HttpClient(Address);

            ServicePointManager.Expect100Continue = false;

            HttpResponseMessage message;

            if (CustomHeaders.Count > 0)
            {
                foreach (var header in CustomHeaders)
                {
                    if (Client.DefaultHeaders.ContainsKey(header.Key))
                    {
                        Client.DefaultHeaders[header.Key] = header.Value;
                    }
                    else
                    {
                        Client.DefaultHeaders.Add(header.Key, header.Value);
                    }
                }
            }

            switch (method)
            {
                case (HttpVerb.Get):
                    {
                        message = Client.Get(path);
                        break;
                    }

                case (HttpVerb.Post):
                    {
                        message = Client.Post(path, encoding, HttpContent.Create(data));
                        break;
                    }

                case (HttpVerb.Put):
                    {
                        message = Client.Put(path, encoding, HttpContent.Create(data));
                        break;
                    }

                case (HttpVerb.Delete):
                    {
                        message = Client.Delete(path);
                        break;
                    }

                default:
                    {
                        return string.Empty;
                    }
            }

            return message.Content.ReadAsString();
        }

        private static string FixHost(string urlString)
        {
            urlString = urlString.TrimEnd(new[] { '/' });

            if (!Uri.IsWellFormedUriString(urlString, UriKind.RelativeOrAbsolute) && !urlString.Contains("://"))
            {
                urlString = "http://" + urlString;
            }

            return urlString;
        }

        #endregion
    }
}