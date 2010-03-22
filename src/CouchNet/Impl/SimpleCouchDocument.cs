using Newtonsoft.Json;

namespace CouchNet.Impl
{
    public class SimpleCouchDocument : ICouchDocument
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_rev")]
        public string Revision { get; set; }

        [JsonProperty(PropertyName = "_deleted")]
        public bool? IsDeleted { get; set; }
    }
}