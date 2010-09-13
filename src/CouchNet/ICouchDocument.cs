namespace CouchNet
{
    public interface ICouchDocument
    {
        string Id { get; set; }
        string Revision { get; }
        //bool? IsDeleted { get; }
    }
}