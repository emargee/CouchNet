using CouchNet.Utils;

namespace CouchNet.Enums
{
    public enum CouchViewMode
    {
        [EnumString("_view")]
        View,
        [EnumString("_show")]
        Show,
        [EnumString("_list")]
        List
    }
}