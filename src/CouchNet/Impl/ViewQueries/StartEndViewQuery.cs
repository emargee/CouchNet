using CouchNet.Utils;

namespace CouchNet.Impl.ViewQueries
{
    public class StartEndViewQuery : BaseViewQuery
    {
        public string StartKey { get; set; }
        public string EndKey { get; set; }

        public override string ToString()
        {
            var qs = new QueryString(base.ToString());

            if (!string.IsNullOrEmpty(StartKey))
            {
                qs.Add("startkey", "\"" + StartKey + "\"");
            }

            if (!string.IsNullOrEmpty(EndKey))
            {
                qs.Add("endkey", "\"" + EndKey + "\"");
            }

            return qs.ToString();
        }
    }
}