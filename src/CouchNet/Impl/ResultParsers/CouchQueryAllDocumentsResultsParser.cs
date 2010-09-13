using System.Linq;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Impl.QueryResults;
using CouchNet.Impl.ServerResponse;
using CouchNet.Internal;
using Newtonsoft.Json;

namespace CouchNet.Impl.ResultParsers
{
    public class CouchQueryAllDocumentsResultsParser<T> : ICouchQueryResultsParser<T> where T : ICouchDocument
    {
        public ICouchQueryResults<T> Parse(IHttpResponse rawResponse)
        {
            var results = new CouchQueryResults<T>();
            var settings = CouchService.JsonSettings;

            if (rawResponse.StatusCode != HttpStatusCode.OK && rawResponse.StatusCode != HttpStatusCode.NotModified)
            {
                if (rawResponse.Data.Contains("\"error\""))
                {
                    results.Response = new CouchServerResponse(JsonConvert.DeserializeObject<CouchServerResponseDefinition>(rawResponse.Data));
                }

                return results;
            }

            var cdbResult = JsonConvert.DeserializeObject<CouchViewResultsDefinition<CouchAllDocsResultRowDefinition<T>>>(rawResponse.Data, settings);

            if (cdbResult != null && cdbResult.Rows.Count() >= 0)
            {
                results.Response = new CouchServerResponse(true);

                foreach (var filteredResult in cdbResult.Rows.SkipWhile(s => s.Value.IsDeleted == true).Select(row => row.Document))
                {
                    results.Add(filteredResult);
                }

                results.TotalRows = cdbResult.TotalRows;
                results.Offset = cdbResult.Offset;
            }

            return results;
        }
    }
}