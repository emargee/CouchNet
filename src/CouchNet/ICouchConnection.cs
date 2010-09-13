using CouchNet.HttpTransport;

namespace CouchNet
{
    public interface ICouchConnection
    {
        ICouchCache Cache { get; set; }

        IHttpResponse Get(string path);
        IHttpResponse Get(string path, string encoding);
        IHttpResponse Put(string path, string data);
        IHttpResponse Put(string path, string data, string encoding);
        IHttpResponse Post(string path, string data);
        IHttpResponse Post(string path, string data, string encoding);
        IHttpResponse Delete(string path);
        IHttpResponse Delete(string path, string encoding);
        IHttpResponse Copy(string fromPath, string newDocId);
        IHttpResponse Copy(string fromPath, string newDocId, string encoding);
        IHttpResponse Head(string path);
    }
}