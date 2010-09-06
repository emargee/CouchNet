using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CouchNet.Exceptions;
using CouchNet.HttpTransport;
using CouchNet.Impl.ResultParsers;
using CouchNet.Impl.ServerResponse;
using CouchNet.Internal;
using CouchNet.Utils;

namespace CouchNet.Impl
{
    public class CouchDesignDocument : ICouchDocument
    {
        private readonly ICouchConnection _connection;
        public string Id { get; set; }
        public string Revision { get; private set; }
        public bool? IsDeleted { get; private set; }

        public string Name { get; set; }
        public string Language { get; set; }

        public int ViewCount
        {
            get { return Views.Count; }
        }

        public int ShowCount
        {
            get { return Shows.Count; }
        }

        public int ListCount
        {
            get { return Lists.Count; }
        }

        private readonly string _parentDatabase;
        private bool HasPendingChanges { get; set; }
        private Dictionary<string,CouchViewDefinition> Views { get; set; }
        private Dictionary<string, string> Shows { get; set; }
        private Dictionary<string, string> Lists { get; set; }
        private CouchDesignDocumentDefinition InternalDocument { get; set; }

        public CouchView this[string viewName]
        {
            get { return View(viewName); }
        }

        internal CouchDesignDocument(CouchDesignDocumentDefinition designDocument, string parentDatabase, ICouchConnection connection)
        {
            _connection = connection;
            InternalDocument = designDocument;
            Id = designDocument.Id;
            Revision = designDocument.Revision;
            Name = designDocument.Id.Replace("_design/", string.Empty);
            Views = designDocument.Views;
            Shows = designDocument.Shows;
            Lists = designDocument.Lists;
            Language = designDocument.Language;
            HasPendingChanges = false;
            _parentDatabase = parentDatabase;
        }

        public ICouchServerResponse BeginCompact()
        {
            var path = string.Format("{0}/_compact/{1}", _parentDatabase, Id);
            var response = _connection.Post(path, null);

            return new CouchServerResponse(response);
        }

        public CouchDesignDocumentInfoResponse Info()
        {
            var path = string.Format("{0}/{1}/_info", _parentDatabase, Id);

            var rawResponse = _connection.Get(path);

            return new CouchDesignDocumentInfoResponse(rawResponse);
        }

        public ICouchQueryResults<T> ExecuteView<T>(string viewName, CouchViewQuery query) where T : ICouchDocument
        {
            return ExecuteView<T>(View(viewName), query);
        }

        public CouchHandlerResponse ExecuteList(string handler, string viewName, CouchViewQuery query)
        {
            return ExecuteList(List(handler), View(viewName), query);
        }

        public CouchHandlerResponse ExecuteShow(string handler)
        {
            return ExecuteShow(Show(handler), null, new NameValueCollection());
        }

        public CouchHandlerResponse ExecuteShow(string handler, NameValueCollection queryString)
        {
            return ExecuteShow(Show(handler), null, queryString);
        }

        public CouchHandlerResponse ExecuteShow(string handler, string documentId)
        {
            return ExecuteShow(Show(handler),documentId, new NameValueCollection());
        }

        public CouchHandlerResponse ExecuteShow(string handler, string documentId, NameValueCollection queryString)
        {
            return ExecuteShow(Show(handler),documentId,queryString);
        }

        public override string ToString()
        {
            return Id;
        }

        private CouchView View(string name)
        {
            var result = Views.Where(x => x.Key.ToLower() == name.ToLower());

            if (result.Count() != 1)
            {
                throw new CouchNetDocumentNotFoundException(name);
            }

            return new CouchView(Name, result.FirstOrDefault());
        }

        private CouchShowHandler Show(string name)
        {
            var result = Shows.Where(x => x.Key.ToLower() == name.ToLower());

            if (result.Count() != 1)
            {
                throw new CouchNetDocumentNotFoundException(name);
            }

            return new CouchShowHandler(Name, result.FirstOrDefault());    
        }

        private CouchListHandler List(string name)
        {
            var result = Lists.Where(x => x.Key.ToLower() == name.ToLower());

            if (result.Count() != 1)
            {
                throw new CouchNetDocumentNotFoundException(name);
            }

            return new CouchListHandler(Name, result.FirstOrDefault());
        }

        #region Private Methods

        private ICouchQueryResults<T> ExecuteView<T>(ICouchView view, CouchViewQuery query) where T : ICouchDocument
        {
            var path = string.Format("{0}/{1}{2}", _parentDatabase, view, query);

            var rawResponse = _connection.Get(path);

            var results = new CouchQueryViewResultsParser<T>().Parse(rawResponse);

            return results;
        }

        private CouchHandlerResponse ExecuteList(CouchListHandler handler, ICouchView view, CouchViewQuery query)
        {
            var path = string.Format("{0}/{1}/{2}{3}", _parentDatabase, handler, view.Name, query);

            var rawResponse = _connection.Get(path);

            return new CouchHandlerResponse(rawResponse);
        }

        private CouchHandlerResponse ExecuteShow(CouchShowHandler handler, string documentId, NameValueCollection queryString)
        {
            var qs = new QueryString();

            if (queryString.HasKeys())
            {
                qs.Add(queryString);
            }

            string path;

            if (string.IsNullOrEmpty(documentId))
            {
                path = string.Format("{0}/{1}{2}", _parentDatabase, handler, qs);
            }
            else
            {
                path = string.Format("{0}/{1}/{2}{3}", _parentDatabase, handler, documentId, qs);
            }

            var rawResponse = _connection.Get(path);

            return new CouchHandlerResponse(rawResponse);
        }

        #endregion
    }
}