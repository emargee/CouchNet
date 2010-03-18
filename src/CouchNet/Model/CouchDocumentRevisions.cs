using Newtonsoft.Json;

namespace CouchNet.Model
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