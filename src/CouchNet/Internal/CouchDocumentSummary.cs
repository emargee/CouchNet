using Newtonsoft.Json;

namespace CouchNet.Internal
{
    [JsonObject]
    internal class CouchDocumentSummary : ICouchDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "rev")]
        public string Revision { get; set; }

        [JsonProperty(PropertyName = "deleted")]
        public bool? IsDeleted { get; set; }

        [JsonProperty(PropertyName = "conflicts")]
        public string[] Conflicts { get; set; }

        [JsonProperty(PropertyName = "deleted_conflicts")]
        public string[] DeletedConflicts { get; set; }
    }
}