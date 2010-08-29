using CouchNet.Utils;

namespace CouchNet.Base
{
    public class BaseViewQuery
    {
        internal int? Limit { get; set; }
        internal int? Skip { get; set; }

        internal bool UseStale { get; set; }
        internal bool SortDescending { get; set; }
        internal bool Group { get; set; }
        internal int? GroupLevel { get; set; }
        internal bool DisableReduce { get; set; }
        internal bool IncludeDocs { get; set; }
        internal bool DisableInclusiveEnd { get; set; }

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

            if (DisableInclusiveEnd)
            {
                qs.Add("inclusive_end", "false");
            }

            return qs.ToString();
        }
    }

}