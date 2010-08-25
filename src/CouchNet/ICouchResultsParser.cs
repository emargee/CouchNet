using CouchNet.HttpTransport;

namespace CouchNet
{
    public interface ICouchResultsParser<T>
    {
        ICouchQueryResults<T> Parse(IHttpResponse rawResponse);
    }
}