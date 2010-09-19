using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using CouchNet.Impl.ResultParsers;
using CouchNet.Impl.ServerResponse;
using CouchNet.Internal;
using CouchNet.Utils;

namespace CouchNet.Impl
{
    public class CouchDesignDocument : ICouchDocument, ITrackChanges
    {
        internal readonly CouchDatabase Database;

        public string Id { get; set; }
        public string Revision { get; private set; }

        public readonly string Name;
        public string Language { get; set; }

        public bool HasPendingChanges { get; set; }

        public IDictionary<string,CouchView> Views { get; set; }
        public IDictionary<string,CouchShowHandler> Shows { get; set; }
        public IDictionary<string,CouchListHandler> Lists { get; set; }
        public IDictionary<string,CouchDocumentUpdateHandler> DocumentUpdaters { get; set; }

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
            Views = new Dictionary<string, CouchView>();
            Shows = new Dictionary<string, CouchShowHandler>();
            Lists = new Dictionary<string, CouchListHandler>();
            HasPendingChanges = true;
        }

        internal CouchDesignDocument(CouchDesignDocumentDefinition designDocument, CouchDatabase database)
        {
            Database = database;

            Id = designDocument.Id;
            Revision = designDocument.Revision;
            Name = designDocument.Id.Replace("_design/", string.Empty);
            Views = designDocument.Views.ToDictionary(k => k.Key, v => new CouchView(v,this));
            Shows = designDocument.Shows.ToDictionary(k => k.Key, v => new CouchShowHandler(v,this));
            Lists = designDocument.Lists.ToDictionary(k => k.Key, v => new CouchListHandler(v, this));
            DocumentUpdaters = designDocument.DocumentUpdateHandlers.ToDictionary(k => k.Key, v => new CouchDocumentUpdateHandler(v, this));
            Language = designDocument.Language;
            HasPendingChanges = false;
        }

        #endregion

        #region Views

        public CouchView CreateView(string name)
        {
            var view = new CouchView(name, this);
            Views.Add(name, view);
            return view;
        }

        public ICouchQueryResults<T> ExecuteView<T>(string viewName, CouchViewQuery query) where T : ICouchDocument
        {
            return ExecuteView<T>(Views[viewName], query);
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

        public CouchShowHandler CreateShow(string name)
        {
            var show = new CouchShowHandler(name, this);
            Shows.Add(name, show);
            return show;
        }

        public CouchHandlerResponse ExecuteShow(string handler)
        {
            return ExecuteShow(Shows[handler], null, new NameValueCollection());
        }

        public CouchHandlerResponse ExecuteShow(string handler, NameValueCollection queryString)
        {
            return ExecuteShow(Shows[handler], null, queryString);
        }

        public CouchHandlerResponse ExecuteShow(string handler, string documentId)
        {
            return ExecuteShow(Shows[handler], documentId, new NameValueCollection());
        }

        public CouchHandlerResponse ExecuteShow(string handler, string documentId, NameValueCollection queryString)
        {
            return ExecuteShow(Shows[handler], documentId, queryString);
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

        public CouchListHandler CreateList(string name)
        {
            var list = new CouchListHandler(name, this);
            Lists.Add(name, list);
            return list;
        }

        public CouchHandlerResponse ExecuteList(string handler, string viewName, CouchViewQuery query)
        {
            return ExecuteList(Lists[handler], Views[viewName], query);
        }

        public CouchHandlerResponse ExecuteList(CouchListHandler handler, CouchView view, CouchViewQuery query)
        {
            var path = string.Format("{0}/{1}/{2}{3}", Database.Name, handler, view.Name, query);

            var rawResponse = Database.Service.Connection.Get(path);

            return new CouchHandlerResponse(rawResponse);
        }

        #endregion

        #region Updaters

        public CouchDocumentUpdateHandler CreateDocumentUpdater(string name)
        {
            var updater = new CouchDocumentUpdateHandler(name, this);
            DocumentUpdaters.Add(name, updater);
            return updater;
        }

        public CouchHandlerResponse ExecuteDocumentUpdater(CouchDocumentUpdateHandler handler, string documentId, NameValueCollection queryString)
        {
            //201 created + output
            throw new NotImplementedException();
        }

        #endregion

        public override string ToString()
        {
            return Id;
        }
    }
}