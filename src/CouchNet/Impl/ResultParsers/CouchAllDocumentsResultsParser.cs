using System.Linq;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Impl.QueryResults;
using CouchNet.Impl.ServerResponse;
using CouchNet.Internal;
using Newtonsoft.Json;

namespace CouchNet.Impl.ResultParsers
{
    public class CouchAllDocumentsResultsParser<T> : ICouchResultsParser<T> where T : ICouchDocument
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public ICouchQueryResults<T> Parse(IHttpResponse rawResponse)
        {
            var results = new CouchQueryResults<T>();

            if (rawResponse.StatusCode != HttpStatusCode.OK)
            {
                if (rawResponse.Data.Contains("\"error\""))
                {
                    results.Response = new CouchServerResponse(JsonConvert.DeserializeObject<CouchRawServerResponse>(rawResponse.Data));
                }

                return results;
            }

            var cdbResult = JsonConvert.DeserializeObject<CouchViewResults<CouchAllDocsResultRow<T>>>(rawResponse.Data, _settings);

            foreach(var filteredResult in cdbResult.Rows.SkipWhile(s => s.Value.IsDeleted == true).Select(row => row.Document))
            {
                results.Add(filteredResult);
            }

            return results;
        }
    }
}