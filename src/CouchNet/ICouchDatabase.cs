using CouchNet.Enums;
using CouchNet.Model;

namespace CouchNet
{
    public interface ICouchDatabase
    {
        CouchServerResponse ServerResponse { get; }     
        CouchDatabaseStatus Status();

        T Get<T>(string id) where T : ICouchDocument;
        T Get<T>(string id, string revision) where T : ICouchDocument;
        T Get<T>(string id, CouchDocumentOptions options) where T : ICouchDocument;
        void Add(ICouchDocument document);
        void Save(ICouchDocument document);
        void Delete(ICouchDocument document);
        void Delete(string id, string revision);
    }
}