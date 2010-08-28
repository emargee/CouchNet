using System;
using System.Linq;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Impl.QueryResults;
using CouchNet.Impl.ServerResponse;
using CouchNet.Internal;
using Newtonsoft.Json;

namespace CouchNet.Impl.ResultParsers
{
    public class CouchViewResultsParser<T> : ICouchResultsParser<T> where T : ICouchDocument
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

            var cdbResult = JsonConvert.DeserializeObject<CouchViewResults<CouchViewResultsRow<T>>>(rawResponse.Data, _settings);

            if(cdbResult != null && cdbResult.Rows.Count() >= 0)
            {
                results.Response = new CouchServerResponse(true);

                foreach (var row in cdbResult.Rows)
                {
                    results.Add(row.Value);
                }

                results.TotalRows = cdbResult.TotalRows;
                results.Offset = cdbResult.Offset;
            }

            return results;
        }
    }
}