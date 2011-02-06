using System;
using System.Net;
using CouchNet.HttpTransport;
using Moq;
using NUnit.Framework;
using CouchNet.Impl;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchConnectionFixture
    {
        private Mock<IHttpTransportFactory> _httpTransportFactory;
        private Mock<IHttpTransport> _httpTransport;
        private Mock<IHttpResponse> _httpResponse;
        private string _defaultEncoding;

        [SetUp]
        public void Setup()
        {
            _httpTransportFactory = new Mock<IHttpTransportFactory>(MockBehavior.Strict);
            _httpTransport = new Mock<IHttpTransport>(MockBehavior.Strict);
            _httpTransportFactory.Setup(x => x.Create(It.IsAny<Uri>())).Returns(_httpTransport.Object);
            _httpResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _httpResponse.SetupAllProperties();

            _defaultEncoding = "application/json";
        }

        #region Constructor

        [Test]
        public void Ctor_Empty_CanCreateLocalUrl()
        {
            var ch = new CouchConnection();
            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(5984, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_FromUri_CanCreate()
        {
            var ch = new CouchConnection(new Uri("http://localhost"));
            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreate()
        {
            var ch = new CouchConnection("http://localhost");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreateHttps()
        {
            var ch = new CouchConnection("https://localhost");

            Assert.AreEqual("https", ch.BaseAddress.Scheme);
            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(443, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreateWithoutHttp()
        {
            var ch = new CouchConnection("localhost");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreateWithEndSlash()
        {
            var ch = new CouchConnection("localhost/");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreatewithPortInString()
        {
            var ch = new CouchConnection("http://localhost:80/");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_UrlandPort_CanCreate()
        {
            var ch = new CouchConnection("http://localhost/", 1234);

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(1234, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_UrlandPort_WithoutHttp()
        {
            var ch = new CouchConnection("localhost", 1234);

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(1234, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_UrlandPort_WithTrailingSlash()
        {
            var ch = new CouchConnection("localhost/", 1234);

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(1234, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_Full_CanCreate()
        {
            var ch = new CouchConnection("http://localhost/", 1234, "application/xml");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(1234, ch.BaseAddress.Port);
            Assert.AreEqual("application/xml", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_Login_SetsCredentials()
        {
            var ch = new CouchConnection("http://user:pass@localhost/");

            Assert.AreEqual("Basic dXNlcjpwYXNz", ch.Transport.GetHeader("Authorization"));
        }

        [Test]
        public void Ctor_Login_DiscardsWithoutPassword()
        {
            var ch = new CouchConnection("http://userpass@localhost/");

            Assert.IsNullOrEmpty(ch.Transport.GetHeader("Authorization"));
        }

        #endregion

        #region Header Manipulation

        [Test]
        public void Header_SetHeader_AppearsInCollection()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.SetHeader("User-Agent", "user1");

            Assert.AreEqual(1, ch.Transport.HeaderCount());
        }

        [Test]
        public void Header_Duplicate_ShouldOverwrite()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.SetHeader("User-Agent", "user1");
            ch.SetHeader("User-Agent", "user2");

            Assert.AreEqual(1, ch.Transport.HeaderCount());
            Assert.AreEqual("user2", ch.Transport.GetHeader("User-Agent"));
        }

        [Test]
        public void Header_Nulls_DoesNothing()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.SetHeader(null,null);

            Assert.AreEqual(0, ch.Transport.HeaderCount());
        }

        [Test]
        public void Header_ClearHeader_ShouldBeZero()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.SetHeader("User-Agent", "user1");
            ch.SetHeader("User-Agent", "user2");
            ch.ClearHeaders();

            Assert.AreEqual(0, ch.Transport.HeaderCount());
        }

        [Test]
        public void Header_SetCredentials_EncodesCorrectly()
        {
            var ch = new CouchConnection("http://localhost");
            ch.SetCredentials("user","pass");

            Assert.AreEqual("Basic dXNlcjpwYXNz",ch.Transport.GetHeader("Authorization"));
        }

        [Test]
        public void Header_SetCredentials_UsingNetworkCredential()
        {
            var ch = new CouchConnection("http://localhost");
            ch.SetCredentials(new NetworkCredential("user","pass"));

            Assert.AreEqual("Basic dXNlcjpwYXNz", ch.Transport.GetHeader("Authorization"));
        }


        [Test]
        public void Header_DisableCache_SetsCorrectly()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.DisableCache();

            Assert.AreEqual("no-cache", ch.Transport.GetHeader("Cache-Control"));
        }

        #endregion

        #region Http Operations

        [Test]
        public void HttpOperation_Get()
        {
            var testPath = "/";

            _httpTransport.Setup(x => x.Send(testPath, HttpVerb.Get, null, _defaultEncoding)).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Get(testPath);

            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Get_WithEncoding()
        {
            var testPath = "/";

            _httpTransport.Setup(x => x.Send(testPath, HttpVerb.Get, null, "application/xml")).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Get(testPath,"application/xml");

            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Put()
        {
            var testPath = "unittest/_revs_limit";
            var testData = "1500";

            _httpTransport.Setup(x => x.Send(testPath, HttpVerb.Put, testData, _defaultEncoding)).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Put(testPath, testData);
        
            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Put_WithEncoding()
        {
            var testPath = "unittest/_revs_limit";
            var testData = "1500";

            _httpTransport.Setup(x => x.Send(testPath, HttpVerb.Put, testData, "application/xml")).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Put(testPath, testData, "application/xml");

            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Post()
        {
            var testPath = "unittest/_bulk_docs";
            var testData = "{\"docs\":[{\"name\":\"jim\",\"age\":0,\"isAlive\":false,\"_id\":\"0\"},{\"name\":\"billy\",\"age\":0,\"isAlive\":false,\"_id\":\"1\"}]}";;

            _httpTransport.Setup(x => x.Send(testPath, HttpVerb.Post, testData, _defaultEncoding)).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Post(testPath, testData);

            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Post_WithEncoding()
        {
            var testPath = "unittest/_bulk_docs";
            var testData = "{\"docs\":[{\"name\":\"jim\",\"age\":0,\"isAlive\":false,\"_id\":\"0\"},{\"name\":\"billy\",\"age\":0,\"isAlive\":false,\"_id\":\"1\"}]}"; ;

            _httpTransport.Setup(x => x.Send(testPath, HttpVerb.Post, testData, "application/xml")).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Post(testPath, testData, "application/xml");

            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Delete()
        {
            var testPath = "/";

            _httpTransport.Setup(x => x.Send(testPath, HttpVerb.Delete, null, _defaultEncoding)).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Delete(testPath);

            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Delete_WithEncoding()
        {
            var testPath = "/";

            _httpTransport.Setup(x => x.Send(testPath, HttpVerb.Delete, null, "application/xml")).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Delete(testPath, "application/xml");

            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Copy()
        {
            var toPath = "/";
            var fromPath = "/";

            _httpTransport.Setup(x => x.Send(toPath, HttpVerb.Copy, fromPath, _defaultEncoding)).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Copy(toPath, fromPath);

            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Copy_WithEncoding()
        {
            var toPath = "/";
            var fromPath = "/";

            _httpTransport.Setup(x => x.Send(toPath, HttpVerb.Copy, fromPath, "application/xml")).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Copy(toPath, fromPath, "application/xml");

            Assert.IsNotNull(repsonse);
        }

        [Test]
        public void HttpOperation_Head()
        {
            var testPath = "/";

            _httpTransport.Setup(x => x.Send(testPath, HttpVerb.Head, null, null)).Returns(_httpResponse.Object);

            var connection = new CouchConnection("http://localhost/", _httpTransportFactory.Object);
            var repsonse = connection.Head(testPath);

            Assert.IsNotNull(repsonse);
        }

        #endregion

    }
}