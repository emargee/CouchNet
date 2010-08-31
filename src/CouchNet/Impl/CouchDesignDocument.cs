using System;
using System.Collections.Generic;
using System.Linq;
using CouchNet.Exceptions;
using CouchNet.Internal;

namespace CouchNet.Impl
{
    public class CouchDesignDocument : ICouchDocument
    {
        public string Id { get; set; }
        public string Revision { get; private set; }
        public bool? IsDeleted { get; private set; }

        public string Name { get; set; }
        public string Language { get; set; }

        private bool HasPendingChanges { get; set; }
        private Dictionary<string,CouchViewDefinition> Views { get; set; }
        private Dictionary<string, string> Shows { get; set; }
        private Dictionary<string, string> Lists { get; set; }
        private CouchDesignDocumentDefinition InternalDocument { get; set; }

        internal CouchDesignDocument(CouchDesignDocumentDefinition designDocument)
        {
            InternalDocument = designDocument;
            Id = designDocument.Id;
            Revision = designDocument.Revision;
            Name = designDocument.Id.Replace("_design/", string.Empty);
            Views = designDocument.Views;
            Shows = designDocument.Shows;
            Lists = designDocument.Lists;
            Language = designDocument.Language;
            HasPendingChanges = false;
        }

        public CouchView View(string name)
        {
            var result = Views.Where(x => x.Key.ToLower() == name.ToLower());

            if (result.Count() != 1)
            {
                throw new CouchNetDocumentNotFoundException(name);
            }

            return new CouchView(Name, result.FirstOrDefault());
        }

        public CouchShowHandler Show(string name)
        {
            var result = Shows.Where(x => x.Key.ToLower() == name.ToLower());

            if (result.Count() != 1)
            {
                throw new CouchNetDocumentNotFoundException(name);
            }

            return new CouchShowHandler(Name, result.FirstOrDefault());    
        }

        public CouchListHandler List(string name)
        {
            var result = Lists.Where(x => x.Key.ToLower() == name.ToLower());

            if (result.Count() != 1)
            {
                throw new CouchNetDocumentNotFoundException(name);
            }

            return new CouchListHandler(Name, result.FirstOrDefault());
        }
    }
}