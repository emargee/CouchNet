using System.Net;

namespace CouchNet.Impl
{
    public class CouchResponseMessage : ICouchResponseMessage
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; }
        public string ETag { get; set; }
        public string Content { get; set; }

        public CouchResponseMessage()
        {
            StatusCode = HttpStatusCode.InternalServerError;
            ContentType = string.Empty;
            ETag = string.Empty;
            Content = string.Empty;
        }
    }
}