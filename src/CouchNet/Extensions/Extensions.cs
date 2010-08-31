using CouchNet.Enums;
using CouchNet.Impl;

namespace CouchNet
{
    public static class Extensions
    {
        public static bool IsOptionSelected(this CouchDocumentOptions currentOption, CouchDocumentOptions option)
        {
            return (currentOption & option) != CouchDocumentOptions.None;
        }

        public static CouchViewQuery Key(this CouchViewQuery query, object key)
        {
            query.Key = key;
            return query;
        }

        public static CouchViewQuery EndKey(this CouchViewQuery query, object endKey)
        {
            query.EndKey = endKey;
            return query;
        }

        public static CouchViewQuery StartDocId(this CouchViewQuery query, object startDocId)
        {
            query.StartDocId = startDocId;
            return query;
        }

        public static CouchViewQuery EndDocId(this CouchViewQuery query, object endDocId)
        {
            query.EndDocId = endDocId;
            return query;
        }

        public static CouchViewQuery Limit(this CouchViewQuery query, int limit)
        {
            query.Limit = limit;
            return query;
        }

        public static CouchViewQuery DisableInclusiveEnd(this CouchViewQuery query)
        {
            query.DisableInclusiveEnd = true;
            return query;
        }

        public static CouchViewQuery IncludeDocs(this CouchViewQuery query)
        {
            query.IncludeDocs = true;
            return query;
        }

        public static CouchViewQuery DisableReduce(this CouchViewQuery query)
        {
            query.DisableReduce = true;
            return query;
        }

        public static CouchViewQuery Skip(this CouchViewQuery query, int skip)
        {
            query.Skip = skip;
            return query;
        }
    }
}