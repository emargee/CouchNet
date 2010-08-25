using System.Collections.Generic;
using Newtonsoft.Json;

namespace CouchNet.Internal
{
    [JsonObject]
    internal class CouchDocumentCollection<T> where T : ICouchDocument
    {
        private readonly List<T> _documents;

        public CouchDocumentCollection()
        {
            _documents = new List<T>();
        }

        public CouchDocumentCollection(IEnumerable<T> collection)
        {
            _documents = new List<T>(collection);
        }

        [JsonProperty(PropertyName = "all_or_nothing")]
        public bool? AllOrNothing { get; set; }

        [JsonProperty(PropertyName = "docs")]
        public List<T> Documents 
        {
            get { return _documents; }
        }
    }
}