using System;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.HttpTransport.Impl;
using CouchNet.Impl;
using Moq;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchCacheFixture
    {
        private Mock<IHttpResponse> _cacheExample;

        [SetUp]
        public void Setup()
        {
            _cacheExample = new Mock<IHttpResponse>(MockBehavior.Strict);
            _cacheExample.Setup(x => x.Data).Returns("{\"_id\":\"d1d2bac2b4e65baf10be20bf080009d1\",\"_rev\":\"4-c469370486eb9bd694f42617c63f3656\",\"tags\":[\"apple\",\"orange\"],\"name\":\"apple\"}");
            _cacheExample.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
        }

        [Test]
        public void CouchCache_EmulateCacheHit_ReturnsCachedData()
        {
            var _factory = new Mock<IHttpTransportFactory>(MockBehavior.Strict);
            var _transport = new Mock<IHttpTransport>(MockBehavior.Strict);
            var _cache = new Mock<ICouchCache>(MockBehavior.Strict);
            var _response = new HttpResponse();

            _response.ETag = "1234";
            _response.StatusCode = HttpStatusCode.NotModified;
            _response.Data = "There should be no data (as its a 304)";

            _factory.Setup(x => x.Create(new UriBuilder("http://localhost:5984/").Uri)).Returns(_transport.Object);

            _cache.Setup(x => x["/integrationtest/d1d2bac2b4e65baf10be20bf08000189"])
                .Returns(new CouchCacheEntry("/integrationtest/d1d2bac2b4e65baf10be20bf08000189", "1234", "I am data"));

            _transport.Setup(x => x.CacheMatch("1234"));
            _transport.Setup(x => x.Send("/integrationtest/d1d2bac2b4e65baf10be20bf08000189", HttpVerb.Get, null, "application/json")).Returns(_response);

            var conn = new CouchConnection("http://localhost:5984/", _factory.Object);
            conn.Cache = _cache.Object;

            var resp = conn.Get("/integrationtest/d1d2bac2b4e65baf10be20bf08000189");

            Assert.AreEqual("I am data", resp.Data);
        }

        [Test]
        public void CouchCache_EmulateEmpty_AddsToCacheCorrectly()
        {
            var _factory = new Mock<IHttpTransportFactory>(MockBehavior.Strict);
            var _transport = new Mock<IHttpTransport>(MockBehavior.Strict);
            var _cache = new Mock<ICouchCache>(MockBehavior.Strict);
            var _response = new HttpResponse();

            _response.ETag = "1234";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Data = "I am data";

            _factory.Setup(x => x.Create(new UriBuilder("http://localhost:5984/").Uri)).Returns(_transport.Object);

            _cache.Setup(x => x["/integrationtest/d1d2bac2b4e65baf10be20bf08000189"]).Returns(() => null);
            _cache.Setup(x => x.Add(It.IsAny<CouchCacheEntry>()));

            _transport.Setup(x => x.Send("/integrationtest/d1d2bac2b4e65baf10be20bf08000189", HttpVerb.Get, null, "application/json")).Returns(_response);

            var conn = new CouchConnection("http://localhost:5984/", _factory.Object);
            conn.Cache = _cache.Object;

            var resp = conn.Get("/integrationtest/d1d2bac2b4e65baf10be20bf08000189");

            Assert.AreEqual("I am data", resp.Data);
        }
    }
}