using Newtonsoft.Json;

namespace CouchNet.Internal
{
    [JsonObject]
    public class CouchRewriteHandlerDefinition
    {
        [JsonProperty(PropertyName = "from")]
        public string From { get; set; }

        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }  
    }
}