using System;
using CouchNet.Utils;
using Newtonsoft.Json;

namespace CouchNet
{
    public class CouchViewQuery
    {
        internal object Key { get; set; }
        internal object EndKey { get; set; }
        internal object StartDocId { get; set; }
        internal object EndDocId { get; set; }

        internal int? Limit { get; set; }
        internal int? Skip { get; set; }

        internal bool UseStale { get; set; }
        internal bool SortDescending { get; set; }
        internal bool Group { get; set; }
        internal int? GroupLevel { get; set; }
        internal bool DisableReduce { get; set; }
        internal bool IncludeDocs { get; set; }
        internal bool DisableInclusiveEnd { get; set; }

        internal object StartKey { get; set; }

        public CouchViewQuery() { }

        public CouchViewQuery(object key)
        {
            Key = key;
        }

        public CouchViewQuery(object key, object endKey)
        {
            Key = key;
            EndKey = endKey;
        }

        public override string ToString()
        {
            var qs = new QueryString();

            if (Limit.HasValue)
            {
                qs.Add("limit", Limit.Value.ToString());
            }

            if (Skip.HasValue)
            {
                qs.Add("skip", Skip.Value.ToString());
            }

            if (UseStale)
            {
                qs.Add("stale", "ok");
            }

            if (SortDescending)
            {
                qs.Add("descending", "true");
            }

            if (Group)
            {
                qs.Add("group", "true");
            }

            if (GroupLevel.HasValue)
            {
                qs.Add("group_level", GroupLevel.Value.ToString());
            }

            if (DisableReduce)
            {
                qs.Add("reduce", "false");
            }

            if (IncludeDocs)
            {
                qs.Add("include_docs", "true");
            }

            if (DisableInclusiveEnd)
            {
                qs.Add("inclusive_end", "false");
            }

            if (Key != null && EndKey != null)
            {
                if (Key.GetType() != EndKey.GetType())
                {
                    throw new ArgumentException("Key types do not match !");
                }

                qs.Add("startkey", JsonConvert.SerializeObject(Key));
                qs.Add("endkey", ProcessWildcard(EndKey));
            }
            else if (Key != null)
            {
                qs.Add("key", JsonConvert.SerializeObject(Key));
            }

            if(StartKey != null)
            {
                qs.Add("startkey", JsonConvert.SerializeObject(StartKey));
            }

            if (StartDocId != null)
            {
                qs.Add("startkey_docid", JsonConvert.SerializeObject(StartDocId));

            }

            if (EndDocId != null)
            {
                qs.Add("endkey_docid", JsonConvert.SerializeObject(EndDocId));
            }

            return qs.ToString();
        }

        #region Private Methods

        private static string ProcessWildcard(object end)
        {
            if (end.GetType().IsArray)
            {
                var objArray = new object[((object[])end).Length];

                ((object[])end).CopyTo(objArray, 0);

                for (int i = 0; i < objArray.Length; i++)
                {
                    if ((string)objArray[i] == "*")
                    {
                        objArray[i] = new object();
                    }
                }

                return (JsonConvert.SerializeObject(objArray));
            }

            if ((end as string) != null && ((string)end).Contains("*"))
            {
                return (JsonConvert.SerializeObject(end).Replace("*", "\\" + "u9999"));
            }

            return (JsonConvert.SerializeObject(end));
        }

        #endregion
    }
}