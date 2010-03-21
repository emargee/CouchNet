namespace CouchNet
{
    public interface ICouchDocument
    {
        string Id { get; set; }
        string Revision { get; set; }
        bool? IsDeleted { get; set; }
    }
}