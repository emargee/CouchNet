using System;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Internal;
using Newtonsoft.Json;

namespace CouchNet
{
    public class CouchDesignDocumentInfoResponse : ICouchServerResponse
    {
        public string Id { get; private set; }
        public string Revision { get; private set; }

        public bool IsOk { get; private set; }
        public string ErrorType { get; private set; }
        public string ErrorMessage { get; private set; }

        public bool IsCompactRunning { get; private set; }
        public int DiskSize { get; private set; }
        public string Language { get; private set; }
        public int PurgeSequence { get; private set; }
        public string Signature { get; private set; }
        public int UpdateSequence { get; private set; }
        public bool IsUpdaterRunning { get; private set; }
        public int WaitingClients { get; private set; }
        public bool WaitingCommit { get; private set; } 

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        internal CouchDesignDocumentInfoResponse(IHttpResponse rawResponse)
        {
            if (rawResponse.StatusCode != HttpStatusCode.OK)
            {
                if (rawResponse.Data.Contains("\"error\""))
                {
                    var resp = JsonConvert.DeserializeObject<CouchServerResponseDefinition>(rawResponse.Data);
                    IsOk = resp.IsOk;
                    ErrorType = resp.Error;
                    ErrorMessage = resp.Reason;
                    return;
                }
            }

            try
            {
                var info = JsonConvert.DeserializeObject<CouchDesignDocumentInfoDefinition>(rawResponse.Data, _settings);

                Id = info.Name;
                Revision = "";
                IsOk = true;
                IsCompactRunning = info.ViewIndexData.IsCompactRunning;
                DiskSize = info.ViewIndexData.DiskSize;
                Language = info.ViewIndexData.Language;
                PurgeSequence = info.ViewIndexData.PurgeSequence;
                Signature = info.ViewIndexData.Signature;
                UpdateSequence = info.ViewIndexData.UpdateSequence;
                IsUpdaterRunning = info.ViewIndexData.IsUpdaterRunning;
                WaitingClients = info.ViewIndexData.WaitingClients;
                WaitingCommit = info.ViewIndexData.WaitingCommit;
            }

            catch (Exception ex)
            {
                if (ex is JsonReaderException)
                {
                    IsOk = false;
                    ErrorType = "CouchNet Deserialization Error";
                    ErrorMessage = "Failed to deserialize server response (" + rawResponse.Data + ") - Extra Info : " + ex.Message;
                }
            }
        }
    }
}