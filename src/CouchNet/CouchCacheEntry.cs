using System;

namespace CouchNet
{
    [Serializable]
    public class CouchCacheEntry
    {
        public string Url { get; private set; }

        public string ETag { get; private set; }

        public string Data { get; private set; }

        public CouchCacheEntry(string url, string eTag, string data) 
        {
            Url = url;
            ETag = eTag;
            Data = data;
        }
    }
}