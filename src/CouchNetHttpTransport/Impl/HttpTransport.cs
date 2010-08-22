using System;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Http;
using Microsoft.Http.Headers;

namespace CouchNet.HttpTransport.Impl
{
    public class HttpTransport : IHttpTransport
    {
        internal HttpClient Client { get; set; }

        public HttpTransport(Uri url)
        {
            Client = new HttpClient(url);
        }

        public HttpTransport(string url)
        {
            Client = new HttpClient(url);
        }

        public int HeaderCount()
        {
            return Client.DefaultHeaders.Count();
        }

        public string GetHeader(string key)
        {
            return Client.DefaultHeaders[key];
        }

        public void ClearHeaders()
        {
            Client.DefaultHeaders.Clear();
        }

        public void AddHeader(string key, string value)
        {
            if (key == null || value == null)
            {
                return;
            }

            if (Client.DefaultHeaders.ContainsKey(key))
            {
                Client.DefaultHeaders[key] = value;
            }
            else
            {
                Client.DefaultHeaders.Add(key, value);
            }
        }

        public void SetCredentials(string username, string password)
        {
            string authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            Client.DefaultHeaders.Authorization = new Credential("Basic", authInfo);
        }

        public IHttpResponse Send(string path, HttpVerb method, string data, string encoding)
        {
            ServicePointManager.Expect100Continue = false;

            HttpResponseMessage message;

            switch (method)
            {
                case (HttpVerb.Get):
                    {
                        message = Client.Get(path);
                        break;
                    }

                case (HttpVerb.Post):
                    {
                        message = Client.Post(path, encoding, HttpContent.Create(data));
                        break;
                    }

                case (HttpVerb.Put):
                    {
                        message = Client.Put(path, encoding, HttpContent.Create(data));
                        break;
                    }

                case (HttpVerb.Delete):
                    {
                        message = Client.Delete(path);
                        break;
                    }

                case (HttpVerb.Copy):
                    {
                        Client.DefaultHeaders.Add("Destination", data);
                        message = Client.Send(new HttpRequestMessage("COPY", path));
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException("Unknown/Unsupported HTTP verb.");
                    }
            }

            var response = new HttpResponse();

            if (message.Content != null)
            {
                response.Data = message.Content.ReadAsString();
                response.ContentType = message.Content.ContentType ?? string.Empty;
            }

            response.StatusCode = message.StatusCode;

            if (message.Headers.ETag != null)
            {
                response.ETag = message.Headers.ETag.Tag;
            }

            return response;
        }
    }
}