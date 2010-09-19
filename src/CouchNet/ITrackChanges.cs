namespace CouchNet
{
    public interface ITrackChanges
    {
        bool HasPendingChanges { get; }
    }
}