
namespace CouchNet
{
    public interface ICouchCache
    {
        CouchCacheEntry this[string url] { get; }
        void Add(CouchCacheEntry e);
    }
}