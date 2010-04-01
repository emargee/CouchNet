using Newtonsoft.Json;

namespace CouchNet.Model
{
    [JsonObject]
    public class CouchDocumentDetails
    {
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