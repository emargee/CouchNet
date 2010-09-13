using System.Collections.Generic;

namespace CouchNet.Impl
{
    public class CouchListHandler : ICouchHandler
    {
        internal readonly CouchDesignDocument DesignDocument;
        public readonly string Name;
        public object Function { get; set; }

        public CouchListHandler(string listName, CouchDesignDocument designDocument)
        {
            Name = listName;
            DesignDocument = designDocument;
        }

        internal CouchListHandler(KeyValuePair<string, string> listDefinition, CouchDesignDocument designDocument)
        {
            DesignDocument = designDocument;
            Name = listDefinition.Key;
            Function = listDefinition.Value;
        }

        public override string ToString()
        {
            return string.Format("_design/{0}/_list/{1}", DesignDocument, Name);
        }
    }
}