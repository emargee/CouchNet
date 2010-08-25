using Newtonsoft.Json;

namespace CouchNet.Impl
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