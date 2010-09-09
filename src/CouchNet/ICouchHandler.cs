namespace CouchNet
{
    public interface ICouchHandler
    {
        string Name { get; set; }
        object Function { get; set; }
        string ToString();
    }
}