using Newtonsoft.Json;

namespace CouchNet
{
    public class CouchDocumentSimple : ICouchDocument
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_rev")]
        public string Revision { get; set; }    
    }
}