using System;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Internal;
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

        internal CouchServerResponse(bool isOk)
        {
            IsOk = isOk;
        }

        internal CouchServerResponse(IHttpResponse response)
        {
            try
            {
                var resp = JsonConvert.DeserializeObject<CouchServerResponseDefinition>(response.Data, CouchService.JsonSettings);

                Id = resp.Id;
                Revision = resp.Revision;

                if(resp.IsOk.HasValue)
                {
                    IsOk = resp.IsOk.Value;
                }
                else
                {
                    IsOk = response.StatusCode == HttpStatusCode.Created;
                }

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

        internal CouchServerResponse(CouchServerResponseDefinition responseDefinition)
        {
            Id = responseDefinition.Id;
            Revision = responseDefinition.Revision;
            IsOk = responseDefinition.IsOk.GetValueOrDefault(false);
            ErrorType = responseDefinition.Error;
            ErrorMessage = responseDefinition.Reason;
        }
    }
}