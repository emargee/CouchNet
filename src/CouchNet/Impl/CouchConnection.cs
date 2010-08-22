using System;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;

using CouchNet.HttpTransport;
using CouchNet.HttpTransport.Impl;

namespace CouchNet.Impl
{
    public class CouchConnection : ICouchConnection
    {
        internal readonly IHttpTransport Transport;

        #region Private Properties

        internal Uri BaseAddress { get; set; }

        internal string RequestEncoding { get; set; }

        #endregion

        #region ctor

        public CouchConnection() : this(new UriBuilder("http", "localhost", 5984).Uri) { }

        public CouchConnection(string url) : this(new UriBuilder(url).Uri) { }

        public CouchConnection(string url, IHttpTransportFactory factory) : this(new UriBuilder(url).Uri, "application/json", factory) { }

        public CouchConnection(string host, int port) : this(new UriBuilder(host) { Port = port }.Uri) { }

        public CouchConnection(string host, int port, string encoding) : this(new UriBuilder(host) { Port = port }.Uri, encoding) { }

        public CouchConnection(Uri uri) : this(uri, "application/json") { }

        public CouchConnection(Uri uri, string encoding) : this(uri, encoding, new HttpTransportFactory()) { }

        public CouchConnection(Uri uri, string encoding, IHttpTransportFactory factory)
        {
            BaseAddress = uri;
            RequestEncoding = encoding;
            Transport = factory.Create(BaseAddress);
        }

        #endregion

        #region Interface Methods

        public IHttpResponse Get(string path)
        {
            return Transport.Send(path, HttpVerb.Get, null, RequestEncoding);
        }

        public IHttpResponse Get(string path, string encoding)
        {
            return Transport.Send(path, HttpVerb.Get, null, encoding);
        }

        public IHttpResponse Put(string path, string data)
        {
            return Transport.Send(path, HttpVerb.Put, data, RequestEncoding);
        }

        public IHttpResponse Put(string path, string data, string encoding)
        {
            return Transport.Send(path, HttpVerb.Put, data, encoding);
        }

        public IHttpResponse Post(string path, string data)
        {
            return Transport.Send(path, HttpVerb.Post, data, RequestEncoding);
        }

        public IHttpResponse Post(string path, string data, string encoding)
        {
            return Transport.Send(path, HttpVerb.Post, data, encoding);
        }

        public IHttpResponse Delete(string path)
        {
            return Transport.Send(path, HttpVerb.Delete, null, RequestEncoding);
        }

        public IHttpResponse Delete(string path, string encoding)
        {
            return Transport.Send(path, HttpVerb.Delete, null, encoding);
        }

        public IHttpResponse Copy(string fromPath, string newDocId)
        {
            return Transport.Send(fromPath, HttpVerb.Copy, newDocId, RequestEncoding);
        }

        public IHttpResponse Copy(string fromPath, string newDocId, string encoding)
        {
            return Transport.Send(fromPath, HttpVerb.Copy, newDocId, encoding);
        }

        #endregion

        #region Headers

        public void ClearHeaders()
        {
            Transport.ClearHeaders();
        }

        public void SetHeader(string key, string value)
        {
            Transport.AddHeader(key, value);
        }

        public void SetCredentials(NetworkCredential credential)
        {
            Transport.SetCredentials(credential.UserName, credential.Password);
        }

        public void SetCredentials(string userName, string password)
        {
            Transport.SetCredentials(userName, password);
        }

        public void DisableCache()
        {
            SetHeader("Cache-Control", "no-cache");
        }

        #endregion
    }
}