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
    public class CouchDesignDocument : ICouchDocument, ITrackChanges
    {
        internal readonly CouchDatabase Database;

        public string Id { get; set; }
        public string Revision { get; private set; }

        public readonly string Name;
        public string Language { get; set; }

        public bool HasPendingChanges { get; set; }

        public IDictionary<string, CouchView> Views { get; set; }
        public IDictionary<string, CouchShowHandler> Shows { get; set; }
        public IDictionary<string, CouchListHandler> Lists { get; set; }
        public IDictionary<string, CouchDocumentUpdateHandler> DocumentUpdaters { get; set; }
        public List<CouchRewriteRule> RewriteRules { get; set; }

        #region ctor

        internal CouchDesignDocument(string name, CouchDatabase database)
        {
            if (string.IsNullOrEmpty(name) || database == null)
            {
                throw new ArgumentNullException();
            }

            Name = name;
            Database = database;
            Id = "_design/" + Name; //TODO: check or escape name
            Views = new Dictionary<string, CouchView>();
            Shows = new Dictionary<string, CouchShowHandler>();
            Lists = new Dictionary<string, CouchListHandler>();
            RewriteRules = new List<CouchRewriteRule>();
            DocumentUpdaters = new Dictionary<string, CouchDocumentUpdateHandler>();
            Language = "javascript";
            HasPendingChanges = true;
        }

        internal CouchDesignDocument(CouchDesignDocumentDefinition designDocument, CouchDatabase database)
        {
            Database = database;

            Id = designDocument.Id;
            Revision = designDocument.Revision;
            Name = designDocument.Id.Replace("_design/", string.Empty);
            Views = designDocument.Views.ToDictionary(k => k.Key, v => new CouchView(v, this));
            Shows = designDocument.Shows.ToDictionary(k => k.Key, v => new CouchShowHandler(v, this));
            Lists = designDocument.Lists.ToDictionary(k => k.Key, v => new CouchListHandler(v, this));
            DocumentUpdaters = designDocument.DocumentUpdateHandlers.ToDictionary(k => k.Key, v => new CouchDocumentUpdateHandler(v, this));
            RewriteRules = designDocument.RewriteRules.Select(x => new CouchRewriteRule(x, this)).ToList();
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

        #region RewriteRules

        public CouchRewriteRule CreateRewriteRule(string to, string from, string method)
        {
            throw new NotImplementedException();
        }

        public CouchRewriteRule CreateRewriteRule(string to, string from, string method, string query)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Saving

        public void SaveChanges()
        {
            foreach (var couchView in Views.Where(couchView => couchView.Value.HasPendingChanges))
            {
                HasPendingChanges = true;
            }

            foreach (var couchShow in Shows.Where(couchShow => couchShow.Value.HasPendingChanges))
            {
                HasPendingChanges = true;
            }

            foreach (var couchList in Shows.Where(couchList => couchList.Value.HasPendingChanges))
            {
                HasPendingChanges = true;
            }

            foreach (var couchUpd in Shows.Where(couchUpd => couchUpd.Value.HasPendingChanges))
            {
                HasPendingChanges = true;
            }

            foreach (var couchdRew in RewriteRules.Where(couchdRew => couchdRew.HasPendingChanges))
            {
                HasPendingChanges = true;
            }

            if (!HasPendingChanges)
            {
                return;
            }

            //Compile object..

            var def = new CouchDesignDocumentDefinition { Id = Id };

            if (!string.IsNullOrEmpty(Revision))
            {
                def.Revision = Revision;
            }

            def.Language = Language;

            foreach (var couchView in Views)
            {
                def.Views.Add(couchView.Key, couchView.Value.ToDefinition());
            }

            if(Views.Count == 0) { def.Views = null; }
            if(Shows.Count == 0) { def.Shows = null; }
            if(Lists.Count == 0) { def.Lists = null; }
            if(DocumentUpdaters.Count == 0) { def.DocumentUpdateHandlers = null; }
            if (RewriteRules.Count == 0) { def.RewriteRules = null; }

            if (string.IsNullOrEmpty(Revision))
            {
                if(Database.Exists(Id))
                {
                    throw new CouchDocumentCreationException(Id, "Cannot create new as design document of same name already exists.");
                }

                var addResponse = Database.Add(def);
                
                if(addResponse.IsOk)
                {
                    Revision = addResponse.Revision;
                }

                return;
            }

            var saveResponse = Database.Save(def);

            if(saveResponse.IsOk)
            {
                Revision = saveResponse.Revision;
            }

            return;
        }

        #endregion

        public override string ToString()
        {
            return Id;
        }
    }
}