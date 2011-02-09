using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Impl;
using CouchNet.Tests.Model;
using Moq;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchDesignDocumentFixture
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
            //                           Get("unittest/_design/_design/example/_view/testView?key=%22apple%22")
            _connectionMock.Setup(x => x.Get("unittest/_design/example")).Returns(_viewDefinition.Object);

            var svc = new CouchService(_connectionMock.Object);

            var db = svc.GetDatabase("unittest");

            var query = new CouchViewQuery().Key("apple");

            var results = db.GetDesignDocument("example").ExecuteView<ExampleEntity>("testView", query);
            Assert.IsTrue(results.IsOk);
            Assert.IsFalse(results.HasResults);
            Assert.AreEqual(0, results.Count);
        }
    }
}