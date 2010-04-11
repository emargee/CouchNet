namespace CouchNet
{
    public interface ICouchConnection
    {
        ICouchResponseMessage Get(string path);
        ICouchResponseMessage Get(string path, string encoding);
        ICouchResponseMessage Put(string path, string data);
        ICouchResponseMessage Put(string path, string data, string encoding);
        ICouchResponseMessage Post(string path, string data);
        ICouchResponseMessage Post(string path, string data, string encoding);
        ICouchResponseMessage Delete(string path);
        ICouchResponseMessage Delete(string path, string encoding);
        ICouchResponseMessage Copy(string fromPath, string newDocId);
        ICouchResponseMessage Copy(string fromPath, string newDocId, string encoding);
    }
}