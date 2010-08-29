using CouchNet.Base;
using CouchNet.Enums;

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

        public static BaseViewQuery Limit(this BaseViewQuery query, int limit)
        {
            query.Limit = limit;
            return query;
        }

        public static BaseViewQuery DisableInclusiveEnd(this BaseViewQuery query)
        {
            query.DisableInclusiveEnd = true;
            return query;
        }

        public static BaseViewQuery IncludeDocs(this BaseViewQuery query)
        {
            query.IncludeDocs = true;
            return query;
        }

        public static BaseViewQuery DisableReduce(this BaseViewQuery query)
        {
            query.DisableReduce = true;
            return query;
        }

        public static BaseViewQuery Skip(this BaseViewQuery query, int skip)
        {
            query.Skip = skip;
            return query;
        }
    }
}