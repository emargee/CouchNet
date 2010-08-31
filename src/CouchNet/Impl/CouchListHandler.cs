using System.Collections.Generic;

namespace CouchNet.Impl
{
    public class CouchListHandler : ICouchHandler
    {
        public string DesignDocument { get; private set; }
        public string Name { get; set; }
        public object Function { get; set; }

        internal CouchListHandler(string designDocument, KeyValuePair<string, string> listDefinition)
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