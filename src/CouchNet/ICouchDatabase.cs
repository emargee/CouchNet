using System.Collections.Generic;
using CouchNet.Enums;
using CouchNet.Model;

namespace CouchNet
{
    public interface ICouchDatabase
    {
        CouchServerResponse ErrorResponse { get; }     
        CouchDatabaseStatus Status();

        T Get<T>(string id) where T : ICouchDocument;
        T Get<T>(string id, string revision) where T : ICouchDocument;
        T Get<T>(string id, CouchDocumentOptions options) where T : ICouchDocument;

        IEnumerable<T> GetSelected<T>(IEnumerable<string> ids) where T : ICouchDocument;
               
        IEnumerable<T> GetAll<T>() where T : ICouchDocument;
        IEnumerable<T> GetAll<T>(int? limit, string startkey, string endkey, bool? descending) where T : ICouchDocument;

        CouchServerResponse Add(ICouchDocument document);
        IEnumerable<CouchServerResponse> Add(IEnumerable<ICouchDocument> document);

        CouchServerResponse Save(ICouchDocument document);
        IEnumerable<CouchServerResponse> Save(IEnumerable<ICouchDocument> document);

        CouchServerResponse Delete(ICouchDocument document);
        CouchServerResponse Delete(string id, string revision);

        CouchServerResponse Copy(string fromId, string toId, string revision);
    }
}