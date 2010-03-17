namespace CouchNet
{
    public interface ICouchConnection
    {
        string Get(string path, string encoding);
        string Put(string path, string data, string encoding);
        string Post(string path, string data, string encoding);
        string Delete(string path, string encoding);
    }
}