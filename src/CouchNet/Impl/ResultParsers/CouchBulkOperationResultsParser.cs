using System;
using System.Collections.Generic;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Impl.QueryResults;
using CouchNet.Impl.ServerResponse;
using Newtonsoft.Json;

namespace CouchNet.Impl.ResultParsers
{
    public class CouchBulkOperationResultsParser : ICouchResultsParser<ICouchServerResponse>
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public ICouchQueryResults<ICouchServerResponse> Parse(IHttpResponse rawResponse)
        {
            var results = new CouchQueryResults<ICouchServerResponse>();

            var cdbResults = JsonConvert.DeserializeObject<IEnumerable<CouchRawServerResponse>>(rawResponse.Data, _settings);

            foreach (var result in cdbResults)
            {
                results.Add(new CouchServerResponse(result));
            }

            return results;
        }
    }
}