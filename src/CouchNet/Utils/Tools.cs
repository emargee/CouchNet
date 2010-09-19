using System.IO;
using Microsoft.Win32;

namespace CouchNet.Utils
{
    public class Tools
    {
        private static string MimeType(string filename)
        {
            var mime = "application/octetstream";

            var ext = Path.GetExtension(filename);

            if(string.IsNullOrEmpty(ext))
            {
                return mime;
            }

            var rk = Registry.ClassesRoot.OpenSubKey(ext.ToLower());

            if (rk != null && rk.GetValue("Content Type") != null)
            {
                mime = rk.GetValue("Content Type").ToString();
            }

            return mime;
        }
    }
}