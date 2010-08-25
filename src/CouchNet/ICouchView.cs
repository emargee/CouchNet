using CouchNet.Enums;

namespace CouchNet
{
    public interface ICouchView
    {
        string DesignDocument { get; }
        string Name { get; }
        CouchViewMode Mode { get; }
    }
}