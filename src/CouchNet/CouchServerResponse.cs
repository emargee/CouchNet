using Newtonsoft.Json;

namespace CouchNet
{
    [JsonObject]
    public class CouchServerResponse
    {
        [JsonProperty(PropertyName = "ok")]
        public bool IsOk { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "rev")]
        public string Revision { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }
    }
}