namespace CouchNet.HttpTransport
{
    public interface IHttpTransport
    {
        void ClearHeaders();
        void AddHeader(string key, string value);
        int HeaderCount();
        string GetHeader(string key);
        void CacheMatch(string value);
        void NoCache();
        void SetCredentials(string username, string password);
        IHttpResponse Send(string path, HttpVerb method, string data, string requestEncoding);
    }
}