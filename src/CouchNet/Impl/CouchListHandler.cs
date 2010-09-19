using System.Collections.Generic;

namespace CouchNet.Impl
{
    public class CouchListHandler : ICouchHandler, ITrackChanges
    {
        internal readonly CouchDesignDocument DesignDocument;
        public readonly string Name;
        public object Function { get; set; }

        public bool HasPendingChanges { get; private set; }

        public CouchListHandler(string listName, CouchDesignDocument designDocument)
        {
            Name = listName;
            DesignDocument = designDocument;
            HasPendingChanges = true;
        }

        internal CouchListHandler(KeyValuePair<string, string> listDefinition, CouchDesignDocument designDocument)
        {
            DesignDocument = designDocument;
            Name = listDefinition.Key;
            Function = listDefinition.Value;
            HasPendingChanges = false;
        }

        public override string ToString()
        {
            return string.Format("_design/{0}/_list/{1}", DesignDocument, Name);
        }
    }
}