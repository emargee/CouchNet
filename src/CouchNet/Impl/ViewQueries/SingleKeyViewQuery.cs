using CouchNet.Utils;

namespace CouchNet.Impl.ViewQueries
{
    public class SingleKeyViewQuery : BaseViewQuery
    {
        public string Key { get; set; }

        public override string ToString()
        {
            var qs = new QueryString(base.ToString());
            
            if (!string.IsNullOrEmpty(Key))
            {
                qs.Add("key", "\"" + Key + "\"");
            }

            return qs.ToString();
        }
    }
}