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
        public IEnumerable<CouchRewriteRuleDefinition> RewriteRules { get; set; }

        [JsonProperty(PropertyName = "updates")]
        public Dictionary<string, string> DocumentUpdateHandlers { get; set; }

        [JsonProperty(PropertyName = "validate_doc_update")]
        public string DocumentUpdateValidation { get; set; }

        public CouchDesignDocumentDefinition()
        {
            Views = new Dictionary<string, CouchViewDefinition>();
            Shows = new Dictionary<string, string>();
            Lists = new Dictionary<string, string>();
            RewriteRules = new List<CouchRewriteRuleDefinition>();
            DocumentUpdateHandlers = new Dictionary<string, string>();
        }
    }
}