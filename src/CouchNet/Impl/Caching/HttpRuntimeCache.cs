using System;
using System.Web;
using System.Web.Caching;

namespace CouchNet.Impl.Caching
{
    public class HttpRuntimeCache : ICouchCache
    {
        public int ExpirationWindow { get; set; }

        private readonly string id = Guid.NewGuid().ToString();

        public HttpRuntimeCache()
        {
            ExpirationWindow = 10;
        }

        public string CacheKey(string url)
        {
            return "couch" + id + url;
        }

        public CouchCacheEntry this[string url]
        {
            get { return HttpRuntime.Cache[CacheKey(url)] as CouchCacheEntry; }
        }

        public void Add(CouchCacheEntry e)
        {
            HttpRuntime.Cache.Insert(CacheKey(e.Url), e, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, ExpirationWindow, 0));
        }    
    }
}