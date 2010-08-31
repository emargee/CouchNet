using CouchNet.HttpTransport;

namespace CouchNet
{
    public interface ICouchQueryResultsParser<T>
    {
        ICouchQueryResults<T> Parse(IHttpResponse rawResponse);
    }
}