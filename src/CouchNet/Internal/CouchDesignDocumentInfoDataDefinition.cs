using Newtonsoft.Json;

namespace CouchNet.Internal
{
    [JsonObject]
    internal class CouchDesignDocumentInfoDataDefinition
    {
        [JsonProperty(PropertyName = "compact_running")]
        public bool IsCompactRunning { get; set; }

        [JsonProperty(PropertyName = "disk_size")]
        public int DiskSize { get; set; }

        [JsonProperty(PropertyName = "javascript")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "purge_seq")]
        public int PurgeSequence { get; set; }

        [JsonProperty(PropertyName = "signature")]
        public string Signature { get; set; }

        [JsonProperty(PropertyName = "update_seq")]
        public int UpdateSequence { get; set; }

        [JsonProperty(PropertyName = "updater_running")]
        public bool IsUpdaterRunning { get; set; }

        [JsonProperty(PropertyName = "waiting_clients")]
        public int WaitingClients { get; set; }

        [JsonProperty(PropertyName = "waiting_commit")]
        public bool WaitingCommit { get; set; } 
    }
}