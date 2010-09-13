namespace CouchNet
{
    public interface ICouchHandler
    {
        object Function { get; set; }
        string ToString();
    }
}