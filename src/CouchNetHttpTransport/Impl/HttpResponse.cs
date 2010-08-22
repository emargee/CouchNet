using System.Net;

namespace CouchNet.HttpTransport.Impl
{
    public class HttpResponse : IHttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; }
        public string ETag { get; set; }
        public string Data { get; set; }

        public HttpResponse()
        {
            StatusCode = HttpStatusCode.InternalServerError;
            ContentType = string.Empty;
            ETag = string.Empty;
            Data = string.Empty;
        }
    }
}