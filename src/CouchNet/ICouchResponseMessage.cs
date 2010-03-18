using System.Net;

namespace CouchNet
{
    public interface ICouchResponseMessage
    {
        HttpStatusCode StatusCode { get; set; }
        string ContentType { get; set; }
        string ETag { get; set; }
        string Content { get; set; }
    }
}