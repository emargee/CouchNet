using System;
using CouchNet.Enums;
using CouchNet.Internal;
using CouchNet.Utils;
using Newtonsoft.Json;

namespace CouchNet.Impl
{
    public class CouchView : ICouchView
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };

        public string DesignDocument { get; private set; }
        public string Name { get; private set; }
        
        public string Langauge { get; private set; }
        public string Map { get; private set; }
        public string Reduce { get; private set; }

        public CouchViewMode Mode { get; private set; }

        public string FullPath { get { return string.Format("_design/{0}/{1}/{2}", DesignDocument, StringEnum.GetString(Mode), Name); } }

        public CouchView(string designDocument, string viewName)
        {
            DesignDocument = designDocument;
            Name = viewName;
            Mode = CouchViewMode.View;
        }
    }
}