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
    public class CouchQueryGeneralResultsParser : ICouchQueryResultsParser<CouchDocument>
    {
        public ICouchQueryResults<CouchDocument> Parse(IHttpResponse rawResponse)
        {
            var results = new CouchQueryResults<CouchDocument>();
            var settings = CouchService.JsonSettings;

            if (rawResponse.StatusCode != HttpStatusCode.OK && rawResponse.StatusCode != HttpStatusCode.NotModified)
            {
                if (rawResponse.Data.Contains("\"error\""))
                {
                    results.Response = new CouchServerResponse(rawResponse);
                }

                return results;
            }

            var cdbResult = JsonConvert.DeserializeObject<CouchViewResultsDefinition<CouchViewResultsRowDefinition<CouchDocumentSummaryDefinition>>>(rawResponse.Data, settings);

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