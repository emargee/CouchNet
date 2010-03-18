using System;
using System.Net;
using CouchNet.Enums;
using CouchNet.Impl;
using CouchNet.Tests.Model;
using Moq;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchDatabaseFixture
    {
        private ExampleEntity _exampleObject;
        private Mock<ICouchConnection> _connectionMock;
        private Mock<ICouchResponseMessage> _basicResponse;
        private Mock<ICouchResponseMessage> _notFoundResponse;
        private Mock<ICouchResponseMessage> _missingFieldResponse;
        private Mock<ICouchResponseMessage> _revisionsResponse;
        private Mock<ICouchResponseMessage> _revisionInfoResponse;

        [SetUp]
        public void Setup()
        {
            _exampleObject = new ExampleEntity
            {
                Id = "abc123",
                Revision = "946B7D1C",
                Age = 23,
                IsAlive = true,
                Name = "Bob Smith"
            };
    
            _basicResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _basicResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _basicResponse.Setup(s => s.Content).Returns("{\"name\":\"Bob Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"946B7D1C\"}");

            _missingFieldResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _missingFieldResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _missingFieldResponse.Setup(s => s.Content).Returns("{\"_id\":\"abc123\",\"_rev\":\"946B7D1C\"}");

            _notFoundResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _notFoundResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _notFoundResponse.Setup(s => s.Content).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _revisionsResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionsResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _revisionsResponse.Setup(s => s.Content).Returns("{\"name\":\"Bob Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"946B7D1C\",\"_revisions\":{\"start\":3,\"ids\":[\"54c962f10a96d251fa5ef6bddc4c98cc\",\"9f7bedc5be50f1e745a7cfa4696507fd\",\"b587febf4b8d4a36493e6a9b41261a4c\"]}}");

            _revisionInfoResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionInfoResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _revisionInfoResponse.Setup(s => s.Content).Returns("{\"name\":\"Bob Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"946B7D1C\",\"_revs_info\":[{\"rev\":\"3-54c962f10a96d251fa5ef6bddc4c98cc\",\"status\":\"available\"},{\"rev\":\"2-9f7bedc5be50f1e745a7cfa4696507fd\",\"status\":\"missing\"},{\"rev\":\"1-b587febf4b8d4a36493e6a9b41261a4c\",\"status\":\"available\"}]}");

        }

        [Test]
        public void Ctor_Create_CreatesCorrectly()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            Assert.IsNotNull(db);
            Assert.IsNull(db.ServerResponse);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Create_ThrowsOnNullName()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Create_ThrowsOnNullConnection()
        {
            var db = new CouchDatabase<ExampleEntity>(null, "unittest");
        }

        [Test]
        public void Get_BasicDocument_SerializesBackToObject()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123")).Returns(_basicResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Get("abc123");

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(_exampleObject.Age, result.Age);
            Assert.AreEqual(_exampleObject.IsAlive, result.IsAlive);
        }

        [Test]
        public void Get_NotFound_ShouldBeNull()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123")).Returns(_notFoundResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Get("abc123");

            Assert.IsNull(result);
            Assert.IsNotNull(db.ServerResponse);
            Assert.AreEqual("not_found",db.ServerResponse.Error);
            Assert.AreEqual("missing",db.ServerResponse.Reason);
        }

        [Test]
        public void Get_FieldsMissing_NotFoundFieldsShouldBeDefault()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123")).Returns(_missingFieldResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Get("abc123");

            Assert.IsNotNull(result);
            Assert.IsNull(result.Name);
            Assert.AreEqual(0,result.Age);
            Assert.False(result.IsAlive);
        }

        [Test]
        public void Get_GetRevision_SerializeNormally()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123?rev=946B7D1C")).Returns(_basicResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Get("abc123", "946B7D1C");

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(_exampleObject.Age, result.Age);
            Assert.AreEqual(_exampleObject.IsAlive, result.IsAlive);
            Assert.AreEqual(_exampleObject.Revision, "946B7D1C");
        }

        [Test]
        public void Get_IncludeRevisions_SerializeNormally()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123?revs=true")).Returns(_revisionsResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Get("abc123",CouchDocumentOptions.IncludeRevisions);

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(3, result.Revisions.Start);
            Assert.AreEqual(3,result.Revisions.RevisionIds.Length);
        }

        [Test]
        public void Get_RevisionInfo_SerializeNormally()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123?revs_info=true")).Returns(_revisionInfoResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Get("abc123", CouchDocumentOptions.RevisionInfo);

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(3, result.RevisionsInfo.Length);
            Assert.AreEqual("available", result.RevisionsInfo[0].Status);
        }

        [Test]
        public void Get_AllOptions_SerializeNormally()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123?revs_info=true&revs=true")).Returns(_revisionInfoResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Get("abc123", CouchDocumentOptions.RevisionInfo|CouchDocumentOptions.IncludeRevisions);

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(3, result.RevisionsInfo.Length);
            Assert.AreEqual("available", result.RevisionsInfo[0].Status);
        }
    }
}