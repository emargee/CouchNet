using System;

namespace CouchNet.Exceptions
{
    public class CouchDatabaseNotFoundException: Exception
    {
        public CouchDatabaseNotFoundException(string documentId) : base("Unable to find the database with the name : [" + documentId + "]") { }
    }
}