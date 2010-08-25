using System.Collections.Generic;

namespace CouchNet
{
    public interface ICouchQueryResults<T> : IList<T>
    {
        ICouchServerResponse Response { get; set; }
        int TotalRows { get; }
        int Offset { get; }
        bool HasResults { get; }
        bool IsOk { get; }
    }
}