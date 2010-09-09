using CouchNet.Enums;

namespace CouchNet
{
    public interface ICouchView
    {
        string Name { get; }
        string Map { get; set; }
        string Reduce { get; set; }
        string ToString();
    }
}