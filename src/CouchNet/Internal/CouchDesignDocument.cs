using System.Collections.Generic;
using CouchNet.Impl;
using Newtonsoft.Json;

namespace CouchNet.Internal
{
    [JsonObject]
    internal class CouchDesignDocument : ICouchDocument
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_rev")]
        public string Revision { get; set; }

        public bool? IsDeleted { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "views")]
        public IEnumerable<CouchViewDefinition> Views { get; set; }
    }
}