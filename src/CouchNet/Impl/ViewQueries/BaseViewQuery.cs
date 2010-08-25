using CouchNet.Utils;

namespace CouchNet.Impl.ViewQueries
{
    public class BaseViewQuery
    {
        public int? Limit { get; set; }
        public int? Skip { get; set; }

        public bool UseStale { get; set; }
        public bool SortDescending { get; set; }
        public bool Group { get; set; }
        public int? GroupLevel { get; set; }
        public bool DisableReduce { get; set; }
        public bool IncludeDocs { get; set; }
        public bool InclusiveEnd { get; set; }

        public override string ToString()
        {
            var qs = new QueryString();

            if (Limit.HasValue)
            {
                qs.Add("limit", Limit.Value.ToString());
            }

            if (Skip.HasValue)
            {
                qs.Add("skip", Skip.Value.ToString());
            }

            if (UseStale)
            {
                qs.Add("stale", "ok");
            }

            if (SortDescending)
            {
                qs.Add("descending", "true");
            }

            if (Group)
            {
                qs.Add("group", "true");
            }

            if (GroupLevel.HasValue)
            {
                qs.Add("group_level", GroupLevel.Value.ToString());
            }

            if (DisableReduce)
            {
                qs.Add("reduce", "false");
            }

            if (IncludeDocs)
            {
                qs.Add("include_docs", "true");
            }

            if (InclusiveEnd)
            {
                qs.Add("inclusive_end", "true");
            }

            return qs.ToString();
        }
    }

}