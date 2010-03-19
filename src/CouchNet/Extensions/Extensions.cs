using CouchNet.Enums;

namespace CouchNet
{
    public static class Extensions
    {
        public static bool IsOptionSelected(this CouchDocumentOptions currentOption, CouchDocumentOptions option)
        {
            return (currentOption & option) != CouchDocumentOptions.None;
        }    
    }
}