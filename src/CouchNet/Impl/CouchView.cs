using System;
using System.Collections.Generic;
using CouchNet.Internal;

namespace CouchNet.Impl
{
    public class CouchView : ICouchView, ITrackChanges
    {
        internal readonly CouchDesignDocument DesignDocument;
        public readonly string Name;        

        public string Map { get; set; }
        public string Reduce { get; set; }

        public bool HasPendingChanges { get; private set; }

        public CouchView(string viewName, CouchDesignDocument designDocument)
        {
            Name = viewName;
            DesignDocument = designDocument;
            HasPendingChanges = true;
        }
        
        internal CouchView(KeyValuePair<string, CouchViewDefinition> viewDefinition, CouchDesignDocument designDocument)
        {
            DesignDocument = designDocument;
            Name = viewDefinition.Key;
            Map = viewDefinition.Value.Map;
            Reduce = viewDefinition.Value.Reduce;
            HasPendingChanges = false;
        }

        public override string ToString()
        {
            return string.Format("{0}/_view/{1}", DesignDocument, Name);
        }
    }
}