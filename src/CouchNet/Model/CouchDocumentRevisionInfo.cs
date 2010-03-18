using Newtonsoft.Json;

namespace CouchNet.Model
{
    [JsonObject]
    public class CouchDocumentRevisionInfo
    {
        [JsonProperty(PropertyName = "rev")]
        public string Revision { get; set; }

        //TODO: Use ENUM
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}