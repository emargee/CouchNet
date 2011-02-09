using System;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Internal;
using Newtonsoft.Json;

namespace CouchNet.Impl.ServerResponse
{
    public class CouchDatabaseStatusResponse : ICouchServerResponse
    {
        public string Id { get; private set; }
        
        public string Revision { get; private set; }

        public bool IsOk { get; private set; }
        
        public string ErrorType { get; private set; }
        
        public string ErrorMessage { get; private set; }

        public string DatabaseName { get; set; }
        
        public int DocumentCount { get; set; }
        
        public int DocumentDeletedCount { get; set; }
        
        public string UpdateSequence { get; set; }
        
        public int PurgeSequence { get; set; }
        
        public bool IsCompactRunning { get; set; }
        
        public int DiskSize { get; set; }
        
        public string InstanceStartTime { get; set; }
        
        public int DiskFormatVersion { get; set; }

        public CouchDatabaseStatusResponse() { }

        internal CouchDatabaseStatusResponse(IHttpResponse rawResponse, JsonSerializerSettings settings)
        {
            if (rawResponse.StatusCode != HttpStatusCode.OK && rawResponse.StatusCode != HttpStatusCode.NotModified)
            {
                if (rawResponse.Data.Contains("\"error\""))
                {
                    var resp = JsonConvert.DeserializeObject<CouchServerResponseDefinition>(rawResponse.Data);
                    IsOk = resp.IsOk.GetValueOrDefault(false);
                    ErrorType = resp.Error;
                    ErrorMessage = resp.Reason;
                    return;
                }
            }

            try
            {
                var status = JsonConvert.DeserializeObject<CouchDatabaseStatusDefinition>(rawResponse.Data, settings);

                Id = status.DatabaseName;
                Revision = status.UpdateSequence;
                IsOk = true;
                DatabaseName = status.DatabaseName;
                DocumentCount = status.DocumentCount;
                DocumentDeletedCount = status.DocumentDeletedCount;
                UpdateSequence = status.UpdateSequence;
                PurgeSequence = status.PurgeSequence;
                IsCompactRunning = status.IsCompactRunning;
                DiskSize = status.DiskSize;
                InstanceStartTime = status.InstanceStartTime;
                DiskFormatVersion = status.DiskFormatVersion;
            }

            catch (Exception ex)
            {
                if (ex is JsonReaderException)
                {
                    IsOk = false;
                    ErrorType = "CouchNet Deserialization Error";
                    ErrorMessage = "Failed to deserialize server response (" + rawResponse.Data + ") - Extra Info : " + ex.Message;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}