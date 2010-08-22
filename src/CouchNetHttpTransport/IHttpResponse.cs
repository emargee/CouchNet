using System.Net;

namespace CouchNet.HttpTransport
{
    public interface IHttpResponse
    {
        HttpStatusCode StatusCode { get; set; }
        string ContentType { get; set; }
        string ETag { get; set; }
        string Data { get; set; }
    }
}