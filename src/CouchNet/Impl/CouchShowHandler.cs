using System.Collections.Generic;

namespace CouchNet.Impl
{
    public class CouchShowHandler : ICouchHandler
    {
        public string DesignDocument { get; private set; }
        public string Name { get; set; }
        public object Function { get; set; }

        internal CouchShowHandler(string designDocument, KeyValuePair<string, string> showDefinition)
        {
            DesignDocument = designDocument;
            Name = showDefinition.Key;
            Function = showDefinition.Value;
        }

        public override string ToString()
        {
            return string.Format("_design/{0}/_show/{1}", DesignDocument, Name);    
        }
    }
}