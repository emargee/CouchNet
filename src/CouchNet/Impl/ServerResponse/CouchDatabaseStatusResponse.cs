using CouchNet.HttpTransport;
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
        public int UpdateSequence { get; set; }
        public int PurgeSequence { get; set; }
        public bool IsCompactRunning { get; set; }
        public int DiskSize { get; set; }
        public string InstanceStartTime { get; set; }
        public int DiskFormatVersion { get; set; }

        public CouchDatabaseStatusResponse() { }

        internal CouchDatabaseStatusResponse(CouchRawServerResponse response)
        {
            IsOk = response.IsOk;
            ErrorType = response.Error;
            ErrorMessage = response.Reason;
        }

        internal CouchDatabaseStatusResponse(IHttpResponse response)
        {
            var status = JsonConvert.DeserializeObject<CouchDatabaseStatus>(response.Data);
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

        internal CouchDatabaseStatusResponse(CouchDatabaseStatus status)
        {
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
    }
}