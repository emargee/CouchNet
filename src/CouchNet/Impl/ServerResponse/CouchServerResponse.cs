using CouchNet.HttpTransport;
using Newtonsoft.Json;

namespace CouchNet.Impl.ServerResponse
{
    public class CouchServerResponse : ICouchServerResponse
    {
        public string Id { get; private set; }
        public string Revision { get; private set; }
        public bool IsOk { get; private set; }
        public string ErrorType { get; private set; }
        public string ErrorMessage { get; private set; }

        internal CouchServerResponse(IHttpResponse response)
        {
            var resp = JsonConvert.DeserializeObject<CouchRawServerResponse>(response.Data);

            Id = resp.Id;
            Revision = resp.Revision;
            IsOk = resp.IsOk;
            ErrorType = resp.Error;
            ErrorMessage = resp.Reason;
        }

        internal CouchServerResponse(CouchRawServerResponse response)
        {
            Id = response.Id;
            Revision = response.Revision;
            IsOk = response.IsOk;
            ErrorType = response.Error;
            ErrorMessage = response.Reason;
        }
    }
}