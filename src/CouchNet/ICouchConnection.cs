namespace CouchNet
{
    public interface ICouchConnection
    {
        ICouchResponseMessage Get(string path, string encoding);
        ICouchResponseMessage Put(string path, string data, string encoding);
        ICouchResponseMessage Post(string path, string data, string encoding);
        ICouchResponseMessage Delete(string path, string encoding);
    }
}