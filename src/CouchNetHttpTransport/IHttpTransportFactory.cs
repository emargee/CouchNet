using System;

namespace CouchNet.HttpTransport
{
    public interface IHttpTransportFactory
    {
        IHttpTransport Create(Uri url);    
    }
}