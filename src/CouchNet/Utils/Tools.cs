using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace CouchNet.Utils
{
    public class Tools
    {
        private static string MimeType(string filename)
        {
            var mime = "application/octetstream";

            var ext = Path.GetExtension(filename);

            if(string.IsNullOrEmpty(ext))
            {
                return mime;
            }

            var rk = Registry.ClassesRoot.OpenSubKey(ext.ToLower());

            if (rk != null && rk.GetValue("Content Type") != null)
            {
                mime = rk.GetValue("Content Type").ToString();
            }

            return mime;
        }
    }

    public sealed class ReadOnlyDictionary<TKey, TValue> : ReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        public ReadOnlyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items) : base(items.ToList()) { }

        public TValue this[TKey key]
        {
            get
            {
                var valueQuery = GetQuery(key);

                if (valueQuery.Count() == 0)
                {
                    throw new NullReferenceException("No value found for given key");
                }

                return valueQuery.First().Value;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return (GetQuery(key).Count() > 0);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var toReturn = ContainsKey(key);
            value = toReturn ? this[key] : default(TValue);
            return toReturn;
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> GetQuery(TKey key)
        {
            return (from t in Items where t.Key.Equals(key) select t);
        }

        internal void InternalAdd(TKey key, TValue value)
        {
            Items.Add(new KeyValuePair<TKey, TValue>(key,value));
        }
    }
}