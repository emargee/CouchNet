using System;
using System.Collections.Generic;
using CouchNet.Internal;

namespace CouchNet.Impl
{
    public class CouchRewriteRule: ITrackChanges
    {
        internal readonly CouchDesignDocument DesignDocument;

        public string From { get; set; }
        public string To { get; set; }
        public string Method { get; set; }
        public string Query { get; set; }

        public CouchRewriteRule(CouchDesignDocument designDocument)
        {
            DesignDocument = designDocument;
            HasPendingChanges = true;
        }

        internal CouchRewriteRule(CouchRewriteRuleDefinition rewriteRuleDefinition, CouchDesignDocument designDocument)
        {
            DesignDocument = designDocument;
            From = rewriteRuleDefinition.From;
            To = rewriteRuleDefinition.To;
            Method = rewriteRuleDefinition.Method;
            Query = rewriteRuleDefinition.Query;
        }

        public bool HasPendingChanges { get; private set; }
    }
}