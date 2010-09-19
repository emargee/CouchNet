using System.Collections.Generic;

namespace CouchNet.Impl
{
    public class CouchDocumentUpdateHandler : ICouchHandler, ITrackChanges
    {
        internal readonly CouchDesignDocument DesignDocument;
        public readonly string Name;
        public object Function { get; set; }

        public bool HasPendingChanges { get; private set; }

        public CouchDocumentUpdateHandler(string viewName, CouchDesignDocument designDocument)
        {
            Name = viewName;
            DesignDocument = designDocument;
            HasPendingChanges = false;
        }

        internal CouchDocumentUpdateHandler(KeyValuePair<string, string> updateDefinition, CouchDesignDocument designDocument)
        {
            DesignDocument = designDocument;
            Name = updateDefinition.Key;
            Function = updateDefinition.Value;
            HasPendingChanges = true;
        }

        public override string ToString()
        {
            return string.Format("_design/{0}/_update/{1}", DesignDocument, Name);    
        }    
    }
}