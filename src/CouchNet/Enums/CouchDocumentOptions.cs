using System;

namespace CouchNet.Enums
{
    [Flags]
    public enum CouchDocumentOptions
    {
        None = 0,
        RevisionInfo = 1,
        IncludeRevisions = 2
    }
}