using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using CouchNet.Exceptions;
using CouchNet.Impl.ResultParsers;
using CouchNet.Impl.ServerResponse;
using CouchNet.Internal;
using CouchNet.Utils;

namespace CouchNet.Impl
{
    public class CouchDesignDocument : ICouchDocument
    {
        internal readonly CouchDatabase Database;

        public string Id { get; set; }
        public string Revision { get; private set; }

        public readonly string Name;
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

        private bool HasPendingChanges { get; set; }
        private Dictionary<string,CouchViewDefinition> Views { get; set; }
        private Dictionary<string, string> Shows { get; set; }
        private Dictionary<string, string> Lists { get; set; }

        #region ctor

        public CouchDesignDocument(string name, CouchDatabase database)
        {
            if(string.IsNullOrEmpty(name) || database == null)
            {
                throw new ArgumentNullException();
            }

            Name = name;
            Database = database;
            Id = "_design/" + Name;
            Views = new Dictionary<string, CouchViewDefinition>();
            Shows = new Dictionary<string, string>();
            Lists = new Dictionary<string, string>();
            HasPendingChanges = true;
        }

        internal CouchDesignDocument(CouchDesignDocumentDefinition designDocument, CouchDatabase database)
        {
            Database = database;

            Id = designDocument.Id;
            Revision = designDocument.Revision;
            Name = designDocument.Id.Replace("_design/", string.Empty);
            Views = designDocument.Views;
            Shows = designDocument.Shows;
            Lists = designDocument.Lists;
            Language = designDocument.Language;
            HasPendingChanges = false;
        }

        #endregion

        #region Views

        public CouchView View(string name)
        {
            var result = Views.Where(x => x.Key.ToLower() == name.ToLower());

            if (result.Count() != 1)
            {
                throw new CouchNetDocumentNotFoundException(name);
            }

            return new CouchView(result.FirstOrDefault(), this);
        }

        public CouchView CreateView(string name)
        {
            return new CouchView(name, this);
        }

        public ICouchQueryResults<T> ExecuteView<T>(string viewName, CouchViewQuery query) where T : ICouchDocument
        {
            return ExecuteView<T>(View(viewName), query);
        }

        public ICouchQueryResults<T> ExecuteView<T>(CouchView view, CouchViewQuery query) where T : ICouchDocument
        {
            var path = string.Format("{0}/{1}{2}", Database.Name, view, query);

            var rawResponse = Database.Service.Connection.Get(path);

            var results = new CouchQueryViewResultsParser<T>().Parse(rawResponse);

            return results;
        }

        #endregion

        #region Shows

        public CouchShowHandler Show(string name)
        {
            var result = Shows.Where(x => x.Key.ToLower() == name.ToLower());

            if (result.Count() != 1)
            {
                throw new CouchNetDocumentNotFoundException(name);
            }

            return new CouchShowHandler(result.FirstOrDefault(),this);        
        }

        public CouchShowHandler CreateShow(string name)
        {
            return new CouchShowHandler(name, this);
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
            return ExecuteShow(Show(handler), documentId, new NameValueCollection());
        }

        public CouchHandlerResponse ExecuteShow(string handler, string documentId, NameValueCollection queryString)
        {
            return ExecuteShow(Show(handler), documentId, queryString);
        }

        public CouchHandlerResponse ExecuteShow(CouchShowHandler handler, string documentId, NameValueCollection queryString)
        {
            var qs = new QueryString();

            if (queryString.HasKeys())
            {
                qs.Add(queryString);
            }

            string path;

            if (string.IsNullOrEmpty(documentId))
            {
                path = string.Format("{0}/{1}{2}", Database.Name, handler, qs);
            }
            else
            {
                path = string.Format("{0}/{1}/{2}{3}", Database.Name, handler, documentId, qs);
            }

            var rawResponse = Database.Service.Connection.Get(path);

            return new CouchHandlerResponse(rawResponse);
        }

        #endregion

        #region Lists

        public CouchListHandler List(string name)
        {
            var result = Lists.Where(x => x.Key.ToLower() == name.ToLower());

            if (result.Count() != 1)
            {
                throw new CouchNetDocumentNotFoundException(name);
            }

            return new CouchListHandler(result.FirstOrDefault(), this);
        }

        public CouchListHandler CreateList(string name)
        {
            return new CouchListHandler(name, this);
        }

        public CouchHandlerResponse ExecuteList(string handler, string viewName, CouchViewQuery query)
        {
            return ExecuteList(List(handler), View(viewName), query);
        }

        public CouchHandlerResponse ExecuteList(CouchListHandler handler, CouchView view, CouchViewQuery query)
        {
            var path = string.Format("{0}/{1}/{2}{3}", Database.Name, handler, view.Name, query);

            var rawResponse = Database.Service.Connection.Get(path);

            return new CouchHandlerResponse(rawResponse);
        }

        #endregion

        public override string ToString()
        {
            return Id;
        }
    }
}