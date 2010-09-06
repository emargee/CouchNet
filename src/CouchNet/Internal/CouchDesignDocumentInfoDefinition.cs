using Newtonsoft.Json;

namespace CouchNet.Internal
{
    [JsonObject]
    internal class CouchDesignDocumentInfoDefinition
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "view_index")]
        public CouchDesignDocumentInfoDataDefinition ViewIndexData { get; set; }
    }
}