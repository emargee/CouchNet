using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Impl;
using CouchNet.Tests.Model;
using Moq;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchDatabaseViewFixture
    {
        private Mock<ICouchConnection> _connectionMock;
        private Mock<IHttpResponse> _viewEmptyResults;

        [SetUp]
        public void Setup()
        {
            _viewEmptyResults = new Mock<IHttpResponse>(MockBehavior.Strict);
            _viewEmptyResults.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _viewEmptyResults.Setup(s => s.Data).Returns("{\"total_rows\":4,\"offset\":2,\"rows\":[]}");
        }

        [Test]
        public void ExecuteView_EmptyResults_ShouldBeOk()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Get("unittest/_design/example/_view/testView?key=%22apple%22")).Returns(_viewEmptyResults.Object);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");

            var view = new CouchView("example", "testView");
            var query = new CouchViewQuery().Key("apple");

            var results = db.ExecuteView<ExampleEntity>(view, query);
            Assert.IsTrue(results.IsOk);
            Assert.IsFalse(results.HasResults);
            Assert.AreEqual(0,results.Count);
        }
    }
}