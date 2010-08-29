using System;
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

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        internal CouchServerResponse(bool isOk)
        {
            IsOk = isOk;
        }

        internal CouchServerResponse(IHttpResponse response)
        {
            try
            {
                var resp = JsonConvert.DeserializeObject<CouchRawServerResponse>(response.Data, _settings);

                Id = resp.Id;
                Revision = resp.Revision;
                IsOk = resp.IsOk;
                ErrorType = resp.Error;
                ErrorMessage = resp.Reason;
            }

            catch(Exception ex)
            {
                if (ex is JsonReaderException)
                {
                    IsOk = false;
                    ErrorType = "CouchNet Deserialization Error";
                    ErrorMessage = "Failed to deserialize server response (" + response.Data + ") Extra Info : " + ex.Message;
                }
            }
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