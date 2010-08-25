namespace CouchNet
{
    public interface ICouchServerResponse
    {
        string Id { get; }
        string Revision { get; }

        bool IsOk { get; }
        string ErrorType { get; }
        string ErrorMessage { get; }
    }
}