using System;
using System.Collections.Generic;
using CouchNet.Internal;

namespace CouchNet.Impl
{
    public class CouchView : ICouchView, ITrackChanges
    {
        internal readonly CouchDesignDocument DesignDocument;
        public readonly string Name;

        private string _map;
        private string _reduce;

        public string Map
        {
            get { return _map; }
            set
            {
                HasPendingChanges = true;
                _map = value;
            }
        }

        public string Reduce
        {
            get { return _reduce; }
            set
            {
                HasPendingChanges = true;
                _reduce = value;
            }
        }

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

        public void SaveChanges()
        {
            DesignDocument.SaveChanges();
        }

        public override string ToString()
        {
            return string.Format("{0}/_view/{1}", DesignDocument, Name);
        }
    }
}