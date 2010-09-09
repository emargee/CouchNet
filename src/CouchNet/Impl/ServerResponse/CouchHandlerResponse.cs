using System.Net;
using CouchNet.HttpTransport;

namespace CouchNet.Impl.ServerResponse
{
    public class CouchHandlerResponse
    {
        public string Output { get; private set; }
        public string ContentType { get; private set; }
        public bool IsOk { get; private set; }

        internal CouchHandlerResponse(IHttpResponse response)
        {
            if(response.StatusCode == HttpStatusCode.OK)
            {
                IsOk = true;
            }

            Output = response.Data;
            ContentType = response.ContentType;
        }
    }
}