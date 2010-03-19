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
        private Mock<ICouchResponseMessage> _statusResponse;
        private Mock<ICouchResponseMessage> _revisionLimitGetResponse;
        private Mock<ICouchResponseMessage> _revisionLimitErrorResponse;
        private Mock<ICouchResponseMessage> _revisionLimitSetResponse;
        private Mock<ICouchResponseMessage> _addDocumentResponse;

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

            _statusResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _statusResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _statusResponse.Setup(s => s.Content).Returns("{\"db_name\":\"unittest\",\"doc_count\":1,\"doc_del_count\":30,\"update_seq\":85,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":53339,\"instance_start_time\":\"1268879146201288\",\"disk_format_version\":4}");

            _revisionLimitGetResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionLimitGetResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _revisionLimitGetResponse.Setup(s => s.Content).Returns("1000");

            _revisionLimitSetResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionLimitSetResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            _revisionLimitSetResponse.Setup(s => s.Content).Returns("1000");

            _revisionLimitErrorResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionLimitErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _revisionLimitErrorResponse.Setup(s => s.Content).Returns("1000");

            _addDocumentResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _addDocumentResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Created);
            _addDocumentResponse.Setup(s => s.Content).Returns("{\"ok\":true, \"id\":\"some_doc_id\", \"rev\":\"2774761002\"}");

        }

        [Test]
        public void Ctor_Create_DbNamesMustBeLowercase()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "UnItTeSt");
            Assert.IsNotNull(db);
            Assert.AreEqual("unittest", db.Name);
        }

        [Test]
        public void Ctor_Create_DbNamesMustEscapeForwardSlash()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "his/her");
            Assert.IsNotNull(db);
            Assert.AreEqual("his%2Fher", db.Name);
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
            new CouchDatabase<ExampleEntity>(_connectionMock.Object, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Create_ThrowsOnNullConnection()
        {
            new CouchDatabase<ExampleEntity>(null, "unittest");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Get_NullRef_ShouldThrow()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            db.Get(null);
        }

        [Test]
        public void Status_GetStatus_ShouldSerializeCorrectly()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest")).Returns(_statusResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Status();

            Assert.AreEqual("unittest", result.DatabaseName);
            Assert.AreEqual(1, result.DocumentCount);
            Assert.AreEqual(30, result.DocumentDeletedCount);
            Assert.AreEqual(85, result.UpdateSequence);
            Assert.AreEqual(0, result.PurgeSequence);
            Assert.AreEqual(false, result.IsCompactRunning);
            Assert.AreEqual(53339, result.DiskSize);
            Assert.AreEqual("1268879146201288", result.InstanceStartTime);
            Assert.AreEqual(4, result.DiskFormatVersion);
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
            Assert.AreEqual("not_found", db.ServerResponse.Error);
            Assert.AreEqual("missing", db.ServerResponse.Reason);
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
            Assert.AreEqual(0, result.Age);
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
            var result = db.Get("abc123", CouchDocumentOptions.IncludeRevisions);

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(3, result.Revisions.Start);
            Assert.AreEqual(3, result.Revisions.RevisionIds.Length);
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
            var result = db.Get("abc123", CouchDocumentOptions.RevisionInfo | CouchDocumentOptions.IncludeRevisions);

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(3, result.RevisionsInfo.Length);
            Assert.AreEqual("available", result.RevisionsInfo[0].Status);
        }

        [Test]
        public void RevisionLimit_Get_ShouldParseValue()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_revs_limit")).Returns(_revisionLimitGetResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            Assert.AreEqual(1000, db.RevisionsLimit);
        }

        [Test]
        public void RevisionLimit_Get_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_revs_limit")).Returns(_revisionLimitErrorResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            Assert.AreEqual(-1, db.RevisionsLimit);
        }

        [Test]
        public void RevisionLimit_Set_ShouldSetValue()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/_revs_limit", "1500")).Returns(_revisionLimitSetResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            db.RevisionsLimit = 1500;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullDocument_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            db.Add(null);
        }

        [Test]
        public void Add_SimpleAdd_CreatesId()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put(It.IsAny<string>(), It.IsAny<string>())).Returns(_addDocumentResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Add(new ExampleEntity { Age = 22, IsAlive = false, Name = "Bob" });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Ok);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual("some_doc_id", result.Id);
        }

        [Test]
        public void Add_SimpleAddWithSpecifiedId_CreatesWithId()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/some_doc_id", It.IsAny<string>())).Returns(_addDocumentResponse.Object);

            var db = new CouchDatabase<ExampleEntity>(_connectionMock.Object, "unittest");
            var result = db.Add(new ExampleEntity { Id = "some_doc_id", Age = 22, IsAlive = false, Name = "Bob" });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Ok);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual("some_doc_id",result.Id);
        }
    }
}