namespace CouchNet
{
    public interface ICouchHandler
    {
        string DesignDocument { get; }
        string Name { get; set; }
        object Function { get; set; }
        string ToString();
    }
}