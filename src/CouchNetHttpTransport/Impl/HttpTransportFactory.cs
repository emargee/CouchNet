using System;

namespace CouchNet.HttpTransport.Impl
{
    public class HttpTransportFactory : IHttpTransportFactory
    {
        public IHttpTransport Create(Uri url)
        {
            return new HttpTransport(url);
        }

        public IHttpTransport Create(string url)
        {
            return new HttpTransport(url);
        }
    }
}