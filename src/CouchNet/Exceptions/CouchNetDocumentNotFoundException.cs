using System;

namespace CouchNet.Exceptions
{
    public class CouchNetDocumentNotFoundException : Exception
    {
        public CouchNetDocumentNotFoundException(string documentId) : base("Unable to find document with the id : [" + documentId + "]") { }
    }
}