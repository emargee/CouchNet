using Newtonsoft.Json;

namespace CouchNet
{
    [JsonObject]
    public class CouchDocumentRevisions
    {
        [JsonProperty(PropertyName = "start")]
        public int Start { get; set; }

        [JsonProperty(PropertyName = "ids")]
        public string[] RevisionIds { get; set; }
    }
}