using System;

namespace CouchNet.Exceptions
{
    public class CouchDocumentSaveException : Exception
    {
        public CouchDocumentSaveException(string id, string errorType, string errorMessage) : base(string.Format("An error of type [{0}] occured while saving document [{1}]. Message : {2}",id, errorType, errorMessage)) { }
    }
}