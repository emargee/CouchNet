using System;
using System.Collections.Generic;
using CouchNet.Internal;
using Newtonsoft.Json;

namespace CouchNet.Impl
{
    public class CouchView : ICouchView
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };

        public string DesignDocument { get; internal set; }
        public string Name { get; internal set; }        

        public string Map { get; set; }
        public string Reduce { get; set; }

        public CouchView() { }
        
        internal CouchView(string designDocument, KeyValuePair<string, CouchViewDefinition> viewDefinition)
        {
            DesignDocument = designDocument;
            Name = viewDefinition.Key;
            Map = viewDefinition.Value.Map;
            Reduce = viewDefinition.Value.Reduce;
        }

        public override string ToString()
        {
            return string.Format("_design/{0}/_view/{1}", DesignDocument, Name);
        }
    }
}