using System;
using CouchNet.Base;
using CouchNet.Utils;
using Newtonsoft.Json;

namespace CouchNet
{
    public class CouchViewQuery : BaseViewQuery
    {
        internal object Key { get; set; }
        internal object EndKey { get; set; }
        internal object StartDocId { get; set; }
        internal object EndDocId { get; set; }

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
            var qs = new QueryString(base.ToString());

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
    }
}