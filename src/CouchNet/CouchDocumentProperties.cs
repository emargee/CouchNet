using Newtonsoft.Json;

namespace CouchNet
{
    public class CouchDocumentProperties
    {
        [JsonProperty(PropertyName = "_deleted")]
        public bool? IsDeleted { get; internal set; }

        [JsonProperty(PropertyName = "_revisions")]
        public CouchDocumentRevisions Revisions { get; internal set; }

        [JsonProperty(PropertyName = "_revs_info")]
        public CouchDocumentRevisionInfo[] RevisionsInfo { get; internal set; }

        [JsonProperty(PropertyName = "_conflicts")]
        public string[] Conflicts { get; internal set; }

        [JsonProperty(PropertyName = "_deleted_conflicts")]
        public string[] DeletedConflicts { get; internal set; }    
    }
}