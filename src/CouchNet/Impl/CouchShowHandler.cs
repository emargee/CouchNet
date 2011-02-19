using System.Collections.Generic;

namespace CouchNet.Impl
{
    public class CouchShowHandler : ICouchHandler, ITrackChanges
    {
        internal readonly CouchDesignDocument DesignDocument;
        public readonly string Name;
        public object Function { get; set; }

        public bool HasPendingChanges { get; private set; }

        public CouchShowHandler(string viewName, CouchDesignDocument designDocument)
        {
            Name = viewName;
            DesignDocument = designDocument;
            HasPendingChanges = true;
        }

        internal CouchShowHandler(KeyValuePair<string, string> showDefinition, CouchDesignDocument designDocument)
        {
            DesignDocument = designDocument;
            Name = showDefinition.Key;
            Function = showDefinition.Value;
            HasPendingChanges = false;
        }

        public override string ToString()
        {
            return string.Format("_design/{0}/_show/{1}", DesignDocument.Name, Name);    
        }
    }
}