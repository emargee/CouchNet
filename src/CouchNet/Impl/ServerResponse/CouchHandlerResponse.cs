using CouchNet.HttpTransport;
using CouchNet.Internal;

namespace CouchNet.Impl.ServerResponse
{
    public class CouchHandlerResponse : CouchServerResponse
    {
        public string Output { get; private set; }
        public string ContentType { get; private set; }

        internal CouchHandlerResponse(bool isOk) : base(isOk) { }

        internal CouchHandlerResponse(IHttpResponse response) : base(response)
        {
            Output = response.Data;
            ContentType = response.ContentType;
        }

        internal CouchHandlerResponse(CouchServerResponseDefinition responseDefinition) : base(responseDefinition) { }
    }
}