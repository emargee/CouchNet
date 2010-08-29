using CouchNet.Enums;
using CouchNet.Internal;
using CouchNet.Utils;
using Newtonsoft.Json;

namespace CouchNet.Impl
{
    public class CouchTempView : ICouchView
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };

        public string DesignDocument { get; private set; }
        public string Name { get; private set; }

        public string Langauge { get; private set; }
        public string Map { get; set; }
        public string Reduce { get; set; }

        public CouchViewMode Mode { get; private set; }

        public string FullPath { get { return string.Format("{0}", StringEnum.GetString(Mode)); } }

        public CouchTempView()
        {
            DesignDocument = string.Empty;
            Name = "_temp_view";
            Mode = CouchViewMode.Temp;
            Langauge = "javascript";
        }

        public override string ToString()
        {
            var temp = new CouchTempViewSubmit { Language = Langauge, Map = Map, Reduce = Reduce };
            return JsonConvert.SerializeObject(temp, Formatting.None, _settings);
        }
    }
}