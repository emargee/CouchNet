using System;

namespace CouchNet.Exceptions
{
    public class CouchDocumentCreationException: Exception
    {
        public CouchDocumentCreationException(string id, string errorMessage) : base(string.Format("Unable to create [{0}]. Message : {1}",id, errorMessage)) { }
    }
}