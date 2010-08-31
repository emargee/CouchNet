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
        private Mock<IHttpResponse> _viewDefinition;

        [SetUp]
        public void Setup()
        {
            _viewEmptyResults = new Mock<IHttpResponse>(MockBehavior.Strict);
            _viewEmptyResults.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _viewEmptyResults.Setup(s => s.Data).Returns("{\"total_rows\":4,\"offset\":2,\"rows\":[]}");

            _viewDefinition = new Mock<IHttpResponse>(MockBehavior.Strict);
            _viewDefinition.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _viewDefinition.Setup(s => s.Data).Returns("{\"_id\":\"_design/example\",\"_rev\":\"1-f1afd087bf27328f6e96291f24b45925\",\"language\":\"javascript\",\"views\":{\"testView\":{\"map\":\"function(doc) {\n  emit(doc._id, doc);\n}\"}}}");
        }

        [Test]
        public void ExecuteView_EmptyResults_ShouldBeOk()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Get("unittest/_design/example/_view/testView?key=%22apple%22")).Returns(_viewEmptyResults.Object);
            _connectionMock.Setup(x => x.Get("unittest/_design/example")).Returns(_viewDefinition.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");

            var view = db.DesignDocument("example").View("testView");
            var query = new CouchViewQuery().Key("apple");

            var results = db.ExecuteView<ExampleEntity>(view, query);
            Assert.IsTrue(results.IsOk);
            Assert.IsFalse(results.HasResults);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void ExecuteView_TempView_CanSerialize()
        {
            var temp = new CouchTempView
                        {
                            Map = "function(doc) { if (doc.foo=='bar') { emit(null, doc.foo); } }",
                            Reduce = "function (key, values, rereduce) { return sum(values); }"
                        };

            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Post("unittest/_temp_view", "{\"language\":\"javascript\",\"map\":\"function(doc) { if (doc.foo=='bar') { emit(null, doc.foo); } }\",\"reduce\":\"function (key, values, rereduce) { return sum(values); }\"}", "application/json")).Returns(_viewEmptyResults.Object);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var resp = db.ExecuteView<CouchDocument>(temp, new CouchViewQuery());
            Assert.IsTrue(resp.IsOk);
            Assert.IsFalse(resp.HasResults);
            Assert.AreEqual(0, resp.Count);
        }
    }
}