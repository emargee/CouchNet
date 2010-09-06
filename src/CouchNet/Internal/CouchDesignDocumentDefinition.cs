using System.Collections.Generic;
using Newtonsoft.Json;

namespace CouchNet.Internal
{
    [JsonObject]
    internal class CouchDesignDocumentDefinition : ICouchDocument
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_rev")]
        public string Revision { get; set; }

        public bool? IsDeleted { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "views")]
        public Dictionary<string,CouchViewDefinition> Views { get; set; }

        [JsonProperty(PropertyName = "shows")]
        public Dictionary<string,string> Shows { get; set; }

        [JsonProperty(PropertyName = "lists")]
        public Dictionary<string, string> Lists { get; set; }

        [JsonProperty(PropertyName = "rewrites")]
        public IEnumerable<CouchRewriteHandlerDefinition> RewriteHandlers { get; set; }

        [JsonProperty(PropertyName = "_updates")]
        public Dictionary<string, string> DocumentUpdateHandlers { get; set; }

        [JsonProperty(PropertyName = "validate_doc_update")]
        public string DocumentUpdateValidation { get; set; }

        public CouchDesignDocumentDefinition()
        {
            Views = new Dictionary<string, CouchViewDefinition>();
            Shows = new Dictionary<string, string>();
            Lists = new Dictionary<string, string>();
            RewriteHandlers = new List<CouchRewriteHandlerDefinition>();
            DocumentUpdateHandlers = new Dictionary<string, string>();
        }
    }
}