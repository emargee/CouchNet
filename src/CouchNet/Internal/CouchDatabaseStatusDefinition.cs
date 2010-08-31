using Newtonsoft.Json;

namespace CouchNet
{
    [JsonObject]
    internal class CouchDatabaseStatusDefinition
    {
        [JsonProperty(PropertyName = "db_name")]
        public string DatabaseName { get; set; }

        [JsonProperty(PropertyName = "doc_count")]
        public int DocumentCount { get; set; }

        [JsonProperty(PropertyName = "doc_del_count")]
        public int DocumentDeletedCount { get; set; }

        [JsonProperty(PropertyName = "update_seq")]
        public int UpdateSequence { get; set; }

        [JsonProperty(PropertyName = "purge_seq")]
        public int PurgeSequence { get; set; }

        [JsonProperty(PropertyName = "compact_running")]
        public bool IsCompactRunning { get; set; }

        [JsonProperty(PropertyName = "disk_size")]
        public int DiskSize { get; set; }

        [JsonProperty(PropertyName = "instance_start_time")]
        public string InstanceStartTime { get; set; }

        [JsonProperty(PropertyName = "disk_format_version")]
        public int DiskFormatVersion { get; set; }
    }
}