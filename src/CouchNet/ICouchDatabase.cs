using CouchNet.Enums;
using CouchNet.Model;

namespace CouchNet
{
    public interface ICouchDatabase<T> where T : CouchDocument
    {
        CouchServerResponse ServerResponse { get; }     
        CouchDatabaseStatus Status();

        T Get(string id);
        T Get(string id, string revision);
        T Get(string id, CouchDocumentOptions options);
    }
}