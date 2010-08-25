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
    public class CouchGeneralResultsParser : ICouchResultsParser<CouchDocument>
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public ICouchQueryResults<CouchDocument> Parse(IHttpResponse rawResponse)
        {
            var results = new CouchQueryResults<CouchDocument>();

            if (rawResponse.StatusCode != HttpStatusCode.OK)
            {
                if (rawResponse.Data.Contains("\"error\""))
                {
                    results.Response = new CouchServerResponse(JsonConvert.DeserializeObject<CouchRawServerResponse>(rawResponse.Data));
                }

                return results;
            }

            var cdbResult = JsonConvert.DeserializeObject<CouchViewResults<CouchViewResultsRow<CouchDocumentSummary>>>(rawResponse.Data, _settings);

            foreach(var result in cdbResult.Rows.Select(row => new CouchDocument { Id = row.Id, Revision = row.Value.Revision, Conflicts = row.Value.Conflicts, IsDeleted = row.Value.IsDeleted, DeletedConflicts = row.Value.DeletedConflicts }))
            {
                results.Add(result);
            }

            return results;
        }
    }
}