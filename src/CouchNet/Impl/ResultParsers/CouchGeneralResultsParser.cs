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
                    results.Response = new CouchServerResponse(rawResponse);
                }

                return results;
            }

            var cdbResult = JsonConvert.DeserializeObject<CouchViewResults<CouchViewResultsRow<CouchDocumentSummary>>>(rawResponse.Data, _settings);

            if (cdbResult != null && cdbResult.Rows.Count() >= 0)
            {
                results.Response = new CouchServerResponse(true);

                foreach (var result in cdbResult.Rows.Select(row => new CouchDocument { Id = row.Id, Revision = row.Value.Revision, Conflicts = row.Value.Conflicts, IsDeleted = row.Value.IsDeleted, DeletedConflicts = row.Value.DeletedConflicts }))
                {
                    results.Add(result);
                }

                results.TotalRows = cdbResult.TotalRows;
                results.Offset = cdbResult.Offset;
            }

            return results;
        }
    }
}