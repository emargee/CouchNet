using System;

namespace CouchNet.Exceptions
{
    public class CouchDocumentNotFoundException : Exception
    {
        public CouchDocumentNotFoundException(string documentId) : base("Unable to find document with the id : [" + documentId + "]") { }
    }
}