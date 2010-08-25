using System;
using CouchNet.Enums;
using CouchNet.Utils;

namespace CouchNet.Impl
{
    public class CouchView : ICouchView
    {
        public string DesignDocument { get; private set; }
        public string Name { get; private set; }
        public CouchViewMode Mode { get; private set; }

        public CouchView(string designDocument, string viewName)
        {
            DesignDocument = designDocument;
            Name = viewName;
            Mode = CouchViewMode.View;
        }

        public override string ToString()
        {
            return string.Format("_design/{0}/{1}/{2}",DesignDocument,StringEnum.GetString(Mode),Name);
        }
    }
}