using System;

namespace CouchNet.Exceptions
{
    public class CouchNetDatabaseNotFoundException: Exception
    {
        public CouchNetDatabaseNotFoundException(string documentId) : base("Unable to find the database with the name : [" + documentId + "]") { }
    }
}