using Newtonsoft.Json;

namespace CouchNet.Model
{
    [JsonObject]
    public class RevisionSegment
    {
        [JsonProperty(PropertyName = "rev")]
        public string Revision { get; set; }
    }
}