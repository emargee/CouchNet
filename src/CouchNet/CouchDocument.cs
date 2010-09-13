using Newtonsoft.Json;

namespace CouchNet
{
    [JsonObject]
    public class CouchDocument : CouchDocumentProperties, ICouchDocument
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_rev")]
        public string Revision { get; set; }
    }
}