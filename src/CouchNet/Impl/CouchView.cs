using System;
using System.Collections.Generic;
using CouchNet.Internal;
using Newtonsoft.Json;

namespace CouchNet.Impl
{
    public class CouchView : ICouchView
    {
        internal readonly CouchDesignDocument DesignDocument;
        public readonly string Name;        

        public string Map { get; set; }
        public string Reduce { get; set; }

        public CouchView(string viewName, CouchDesignDocument designDocument)
        {
            Name = viewName;
            DesignDocument = designDocument;
        }
        
        internal CouchView(KeyValuePair<string, CouchViewDefinition> viewDefinition, CouchDesignDocument designDocument)
        {
            DesignDocument = designDocument;
            Name = viewDefinition.Key;
            Map = viewDefinition.Value.Map;
            Reduce = viewDefinition.Value.Reduce;
        }

        public override string ToString()
        {
            return string.Format("{0}/_view/{1}", DesignDocument, Name);
        }
    }
}