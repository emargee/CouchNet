namespace CouchNet.Impl.Caching
{
    public class NullCache : ICouchCache
    {
        public CouchCacheEntry this[string url]
        {
            get { return null; }
        }

        public void Add(CouchCacheEntry e){ }
    }
}