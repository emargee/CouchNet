using System.Collections.Generic;
using CouchNet.Enums;
using CouchNet.Impl;

namespace CouchNet
{
    public interface ICouchDatabase
    {   
        T Get<T>(string id) where T : ICouchDocument;
        T Get<T>(string id, string revision) where T : ICouchDocument;
        T Get<T>(string id, CouchDocumentOptions options) where T : ICouchDocument;

        ICouchQueryResults<T> GetMany<T>(IEnumerable<string> ids) where T : ICouchDocument;
               
        ICouchQueryResults<T> GetAll<T>() where T : ICouchDocument;
        ICouchQueryResults<T> GetAll<T>(CouchViewQuery query) where T : ICouchDocument;

        ICouchServerResponse Add(ICouchDocument document);
        ICouchQueryResults<ICouchServerResponse> AddMany(IEnumerable<ICouchDocument> document);

        ICouchServerResponse Save(ICouchDocument document);
        ICouchQueryResults<ICouchServerResponse> SaveMany(IEnumerable<ICouchDocument> document);

        ICouchServerResponse Delete(ICouchDocument document);
        ICouchServerResponse Delete(string id, string revision);

        ICouchServerResponse Copy(string fromId, string toId, string revision);

        ICouchQueryResults<T> ExecuteView<T>(ICouchView view, CouchViewQuery query) where T : ICouchDocument;
    }
}