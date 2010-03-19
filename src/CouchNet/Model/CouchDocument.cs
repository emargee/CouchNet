using Newtonsoft.Json;

namespace CouchNet.Model
{
    [JsonObject]
    public class CouchDocument
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_rev")]
        public string Revision { get; set; }

        [JsonProperty(PropertyName = "_deleted")]
        public bool? IsDeleted { get; set; }

        [JsonProperty(PropertyName = "_revisions")]
        public CouchDocumentRevisions Revisions { get; set; }

        [JsonProperty(PropertyName = "_revs_info")]
        public CouchDocumentRevisionInfo[] RevisionsInfo { get; set; }
    }
}