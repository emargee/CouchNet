using CouchNet.Enums;

namespace CouchNet
{
    public interface ICouchView
    {
        string Map { get; set; }
        string Reduce { get; set; }
        string ToString();
    }
}