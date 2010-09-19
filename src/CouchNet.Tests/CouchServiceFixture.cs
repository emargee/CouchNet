using System;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Impl;
using Moq;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchServiceFixture
    {
        private Mock<ICouchConnection> _connectionMock;

        private Mock<IHttpResponse> _successResponse;
        private Mock<IHttpResponse> _errorMissingResponse;


        [SetUp]
        public void Setup()
        {
            _successResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _successResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            _successResponse.Setup(s => s.Data).Returns("{\"ok\":true}");

            _errorMissingResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _errorMissingResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _errorMissingResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");
        }

        [Test]
        public void Compact_Begin_Return()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_compact", null)).Returns(_successResponse.Object);

            var svc = new CouchService(_connectionMock.Object);
            var result = svc.BeginDatabaseCompact(svc.Database("unittest"));

            Assert.IsTrue(result.IsOk);
        }

        [Test]
        public void Compact_NullObject_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_compact", null)).Returns(_errorMissingResponse.Object);

            var svc = new CouchService(_connectionMock.Object);
            Assert.Throws<ArgumentNullException>(() => svc.BeginDatabaseCompact(null));
        }


        //[Test]
        //public void Compact_MissingDb_Throws()
        //{
        //    _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
        //    _connectionMock.Setup(s => s.Post("unittest/_compact", null)).Returns(_errorMissingResponse.Object);

        //    var svc = new CouchService(_connectionMock.Object);
        //    Assert.Throws<ArgumentNullException>(() => svc.BeginDatabaseCompact(svc.Database("unittest")));
        //}

        //[Test]
        //public void ViewCleanup_Begin_Return()
        //{
        //    _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
        //    _connectionMock.Setup(s => s.Post("unittest/_view_cleanup", null)).Returns(_viewCleanResponse.Object);

        //    var db = new CouchDatabase("unittest", _connectionMock.Object);
        //    var result = db.BeginViewCleanup();

        //    Assert.IsTrue(result.IsOk);
        //}

        //[Test]
        //public void ViewCleanup_Error_HandleError()
        //{
        //    _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
        //    _connectionMock.Setup(s => s.Post("unittest/_view_cleanup", null)).Returns(_viewCleanErrorResponse.Object);

        //    var db = new CouchDatabase("unittest", _connectionMock.Object);
        //    var result = db.BeginViewCleanup();

        //    Assert.IsFalse(result.IsOk);
        //}
    }
}