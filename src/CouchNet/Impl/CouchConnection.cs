using System;
using System.Net;
using System.Text;
using Microsoft.Http;

using CouchNet.Enums;
using Microsoft.Http.Headers;

namespace CouchNet.Impl
{
    public class CouchConnection : ICouchConnection
    {
        internal HttpClient Client;

        #region Private Properties

        internal Uri BaseAddress { get; set; }

        internal string RequestEncoding { get; set; }

        #endregion

        #region ctor

        public CouchConnection() : this(new UriBuilder("http", "localhost", 5984).Uri) { }

        public CouchConnection(string url) : this(new UriBuilder(url).Uri) { }

        public CouchConnection(string host, int port) : this(new UriBuilder(host) { Port = port }.Uri) { }

        public CouchConnection(string host, int port, string encoding) : this(new UriBuilder(host) { Port = port }.Uri, encoding) { }

        public CouchConnection(Uri uri) : this(uri, "application/json") { }

        public CouchConnection(Uri uri, string encoding)
        {
            BaseAddress = uri;
            RequestEncoding = encoding;
            Client = new HttpClient(BaseAddress);
        }

        #endregion

        #region Interface Methods

        public ICouchResponseMessage Get(string path)
        {
            return Send(path, HttpVerb.Get, null, RequestEncoding);
        }

        public ICouchResponseMessage Get(string path, string encoding)
        {
            return Send(path, HttpVerb.Get, null, encoding);
        }

        public ICouchResponseMessage Put(string path, string data)
        {
            return Send(path, HttpVerb.Put, data, RequestEncoding);
        }

        public ICouchResponseMessage Put(string path, string data, string encoding)
        {
            return Send(path, HttpVerb.Put, data, encoding);
        }

        public ICouchResponseMessage Post(string path, string data)
        {
            return Send(path, HttpVerb.Post, data, RequestEncoding);
        }

        public ICouchResponseMessage Post(string path, string data, string encoding)
        {
            return Send(path, HttpVerb.Post, data, encoding);
        }

        public ICouchResponseMessage Delete(string path)
        {
            return Send(path, HttpVerb.Delete, null, RequestEncoding);
        }

        public ICouchResponseMessage Delete(string path, string encoding)
        {
            return Send(path, HttpVerb.Delete, null, encoding);
        }

        #endregion

        #region Headers

        public void ClearHeaders()
        {
            Client.DefaultHeaders.Clear();
        }

        public void SetHeader(string key, string value)
        {
            if (key == null || value == null)
            {
                return;
            }

            if (Client.DefaultHeaders.ContainsKey(key))
            {
                Client.DefaultHeaders[key] = value;
            }
            else
            {
                Client.DefaultHeaders.Add(key, value);
            }
        }

        public void SetCredentials(NetworkCredential credential)
        {
            SetCredentials(credential.UserName, credential.Password);
        }

        public void SetCredentials(string userName, string password)
        {
            string authInfo = userName + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            Client.DefaultHeaders.Authorization = new Credential("Basic", authInfo);
        }

        public void DisableCache()
        {
            SetHeader("Cache-Control", "no-cache");
        }

        #endregion

        #region Private Methods

        private ICouchResponseMessage Send(string path, HttpVerb method, string data, string encoding)
        {
            ServicePointManager.Expect100Continue = false;

            HttpResponseMessage message;

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
                        throw new NotImplementedException("Unknown/Unsupported HTTP verb.");
                    }
            }

            var response = new CouchResponseMessage();

            if(message.Content != null)
            {
                response.Content = message.Content.ReadAsString();
                response.ContentType = message.Content.ContentType ?? string.Empty;
            }

            response.StatusCode = message.StatusCode;     

            if(message.Headers.ETag != null)
            {
                response.ETag = message.Headers.ETag.Tag;
            }

            return response;
        }

        #endregion
    }
}