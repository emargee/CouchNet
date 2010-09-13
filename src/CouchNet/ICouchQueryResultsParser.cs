using CouchNet.HttpTransport;
using Newtonsoft.Json;

namespace CouchNet
{
    public interface ICouchQueryResultsParser<T>
    {
        ICouchQueryResults<T> Parse(IHttpResponse rawResponse);
    }
}