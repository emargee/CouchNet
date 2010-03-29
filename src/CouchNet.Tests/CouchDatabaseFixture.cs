using System;
using System.Linq;
using System.Net;
using CouchNet.Enums;
using CouchNet.Impl;
using CouchNet.Model;
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
        private Mock<ICouchResponseMessage> _addConflictDocumentResponse;
        private Mock<ICouchResponseMessage> _revisionLimitCastErrorResponse;
        private Mock<ICouchResponseMessage> _revisionLimitSetErrorResponse;
        private Mock<ICouchResponseMessage> _statusErrorResponse;
        private Mock<ICouchResponseMessage> _compactResponse;
        private Mock<ICouchResponseMessage> _viewCleanResponse;
        private Mock<ICouchResponseMessage> _compactErrorResponse;
        private Mock<ICouchResponseMessage> _viewCleanErrorResponse;
        private Mock<ICouchResponseMessage> _deleteDocumentResponse;
        private Mock<ICouchResponseMessage> _deleteDocumentErrorResponse;
        private Mock<ICouchResponseMessage> _getAllIdsResponse;
        private Mock<ICouchResponseMessage> _getAllIdsErrorResponse;
        private Mock<ICouchResponseMessage> _getAllObjResponse;
        private Mock<ICouchResponseMessage> _getAllObjMixedResponse;

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

            _statusErrorResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _statusErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _statusErrorResponse.Setup(s => s.Content).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _revisionLimitGetResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionLimitGetResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _revisionLimitGetResponse.Setup(s => s.Content).Returns("1000");

            _revisionLimitSetResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionLimitSetResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            _revisionLimitSetResponse.Setup(s => s.Content).Returns("1000");

            _revisionLimitSetErrorResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionLimitSetErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _revisionLimitSetErrorResponse.Setup(s => s.Content).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _revisionLimitErrorResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionLimitErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _revisionLimitErrorResponse.Setup(s => s.Content).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _revisionLimitCastErrorResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _revisionLimitCastErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _revisionLimitCastErrorResponse.Setup(s => s.Content).Returns("Chickens");

            _addDocumentResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _addDocumentResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Created);
            _addDocumentResponse.Setup(s => s.Content).Returns("{\"ok\":true, \"id\":\"some_doc_id\", \"rev\":\"2774761002\"}");

            _addConflictDocumentResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _addConflictDocumentResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Conflict);
            _addConflictDocumentResponse.Setup(s => s.Content).Returns("{\"error\":\"conflict\",\"reason\":\"Document update conflict.\"}");

            _compactResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _compactResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            _compactResponse.Setup(s => s.Content).Returns("{\"ok\":true}");

            _compactErrorResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _compactErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _compactErrorResponse.Setup(s => s.Content).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _viewCleanResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _viewCleanResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            _viewCleanResponse.Setup(s => s.Content).Returns("{\"ok\":true}");

            _viewCleanErrorResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _viewCleanErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _viewCleanErrorResponse.Setup(s => s.Content).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _deleteDocumentResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _deleteDocumentResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _deleteDocumentResponse.Setup(s => s.Content).Returns("{\"ok\":true,\"id\":\"some_id\",\"rev\":\"2-1234\"}");

            _deleteDocumentErrorResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _deleteDocumentErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Conflict);
            _deleteDocumentErrorResponse.Setup(s => s.Content).Returns("{\"error\":\"conflict\",\"reason\":\"Document update conflict.\"}");

            _getAllIdsResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _getAllIdsResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _getAllIdsResponse.Setup(s => s.Content).Returns("{\"total_rows\":2,\"offset\":0,\"rows\":[{\"id\":\"4c51bc81501dd2ee3d20e981a8000562\",\"key\":\"4c51bc81501dd2ee3d20e981a8000562\",\"value\":{\"rev\":\"1-0aaca901d8459d637c4cdc143dc49f65\"}},{\"id\":\"attachment_doc\",\"key\":\"attachment_doc\",\"value\":{\"rev\":\"2-dce2006ce41f3ab6c3e6e3b9e6bca1cb\"}}]}");

            _getAllIdsErrorResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _getAllIdsErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _getAllIdsErrorResponse.Setup(s => s.Content).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _getAllObjResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _getAllObjResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _getAllObjResponse.Setup(s => s.Content).Returns("{\"total_rows\":2,\"offset\":0,\"rows\":[{\"id\":\"abc123\",\"key\":\"abc123\",\"value\":{\"rev\":\"1-946B7D1C\"},\"doc\":{\"name\":\"Fred Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"1-946B7D1C\"} },{\"id\":\"abc456\",\"key\":\"abc456\",\"value\":{\"rev\":\"2-DCE2006C\"},\"doc\":{\"name\":\"Bill Smith\",\"age\":27,\"isAlive\":true,\"_id\":\"abc456\",\"_rev\":\"2-DCE2006C\"} }]}");

            _getAllObjMixedResponse = new Mock<ICouchResponseMessage>(MockBehavior.Strict);
            _getAllObjMixedResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _getAllObjMixedResponse.Setup(s => s.Content).Returns("{\"total_rows\":2,\"offset\":0,\"rows\":[{\"id\":\"abc123\",\"key\":\"abc123\",\"value\":{\"rev\":\"1-946B7D1C\"},\"doc\":{\"name\":\"Fred Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"1-946B7D1C\"} },{\"id\":\"abc456\",\"key\":\"abc456\",\"value\":{\"rev\":\"2-DCE2006C\"},\"doc\":{\"_id\": \"abc456\",\"_rev\": \"2-DCE2006C\",\"Name\": \"Billy Bob\",\"Telephone\": 1234,\"Fax\": 5678} }]}");
        }

        [Test]
        public void Ctor_Create_DbNamesMustBeLowercase()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase(_connectionMock.Object, "UnItTeSt");
            Assert.IsNotNull(db);
            Assert.AreEqual("unittest", db.Name);
        }

        [Test]
        public void Ctor_Create_DbNamesMustEscapeForwardSlash()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase(_connectionMock.Object, "his/her");
            Assert.IsNotNull(db);
            Assert.AreEqual("his%2Fher", db.Name);
        }

        [Test]
        public void Ctor_Create_CreatesCorrectly()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.IsNotNull(db);
            Assert.IsNull(db.ServerResponse);
        }

        [Test]
        public void Ctor_Create_ThrowsOnNullName()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            Assert.Throws<ArgumentNullException>(() => new CouchDatabase(_connectionMock.Object, null));
        }

        [Test]
        public void Ctor_Create_ThrowsOnNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new CouchDatabase(null, "unittest"));
        }

        [Test]
        public void Get_NullRef_ShouldThrow()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Get<ExampleEntity>(null));
        }

        [Test]
        public void Status_Error_ReturnNull()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest")).Returns(_statusErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Status();

            Assert.IsNull(result);
        }

        [Test]
        public void Status_GetStatus_ShouldSerializeCorrectly()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest")).Returns(_statusResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
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
        public void Count_Get_ReturnsValue()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest")).Returns(_statusResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.DocumentCount();

            Assert.AreEqual(1, result);
        }

        [Test]
        public void Count_Error_ReturnsNegative()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest")).Returns(_statusErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.DocumentCount();

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void Get_BasicDocument_SerializesBackToObject()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123")).Returns(_basicResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123");

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

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123");

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

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123");

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

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123", "946B7D1C");

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

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123", CouchDocumentOptions.IncludeRevisions);

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

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123", CouchDocumentOptions.RevisionInfo);

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

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123", CouchDocumentOptions.RevisionInfo | CouchDocumentOptions.IncludeRevisions);

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(3, result.RevisionsInfo.Length);
            Assert.AreEqual("available", result.RevisionsInfo[0].Status);
        }

        [Test]
        public void Get_NullValues_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Get<ExampleEntity>(null, null));

        }

        [Test]
        public void Get_NullValue_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Get<ExampleEntity>(null, CouchDocumentOptions.None));
        }

        [Test]
        public void RevisionLimit_Get_ShouldParseValue()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_revs_limit")).Returns(_revisionLimitGetResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.AreEqual(1000, db.RevisionsLimit);
        }

        [Test]
        public void RevisionLimit_Get_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_revs_limit")).Returns(_revisionLimitErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.AreEqual(-1, db.RevisionsLimit);
        }

        [Test]
        public void RevisionLimit_Get_CastError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_revs_limit")).Returns(_revisionLimitCastErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.AreEqual(0, db.RevisionsLimit);
        }

        [Test]
        public void RevisionLimit_Set_ShouldSetValue()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/_revs_limit", "1500")).Returns(_revisionLimitSetResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.RevisionsLimit = 1500;
        }

        [Test]
        public void RevisionLimit_Set_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/_revs_limit", "1500")).Returns(_revisionLimitSetErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.RevisionsLimit = 1500;
        }

        [Test]
        public void Compact_Begin_Return()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_compact", null)).Returns(_compactResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.BeginCompact();

            Assert.IsTrue(result);
        }

        [Test]
        public void Compact_Error_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_compact", null)).Returns(_compactErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.BeginCompact();

            Assert.IsFalse(result);
            Assert.IsNotNull(db.ServerResponse);
        }

        [Test]
        public void ViewCleanup_Begin_Return()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_view_cleanup", null)).Returns(_viewCleanResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.BeginViewCleanup();

            Assert.IsTrue(result);
        }

        [Test]
        public void ViewCleanup_Error_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_view_cleanup", null)).Returns(_viewCleanErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.BeginViewCleanup();

            Assert.IsFalse(result);
            Assert.IsNotNull(db.ServerResponse);
        }

        [Test]
        public void Add_NullDocument_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Add(null));
        }

        [Test]
        public void Add_SimpleAdd_CreatesId()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put(It.IsAny<string>(), It.IsAny<string>())).Returns(_addDocumentResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.Add(new ExampleEntity { Age = 22, IsAlive = false, Name = "Bob" });

            Assert.IsNotNull(db.ServerResponse);
            Assert.IsTrue(db.ServerResponse.Ok);
            Assert.IsNotNullOrEmpty(db.ServerResponse.Id);
            Assert.AreEqual("some_doc_id", db.ServerResponse.Id);
        }

        [Test]
        public void Add_SimpleAddWithSpecifiedId_CreatesWithId()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/some_doc_id", It.IsAny<string>())).Returns(_addDocumentResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.Add(new ExampleEntity { Id = "some_doc_id", Age = 22, IsAlive = false, Name = "Bob" });

            Assert.IsNotNull(db.ServerResponse);
            Assert.IsTrue(db.ServerResponse.Ok);
            Assert.IsNotNullOrEmpty(db.ServerResponse.Id);
            Assert.AreEqual("some_doc_id", db.ServerResponse.Id);
        }

        [Test]
        public void Add_Conflict_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/some_doc_id", It.IsAny<string>())).Returns(_addConflictDocumentResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.Add(new ExampleEntity { Id = "some_doc_id", Age = 22, IsAlive = false, Name = "Bob" });

            Assert.IsNotNull(db.ServerResponse);
            Assert.IsFalse(db.ServerResponse.Ok);
            Assert.IsNullOrEmpty(db.ServerResponse.Id);
            Assert.AreEqual("conflict", db.ServerResponse.Error);
            Assert.AreEqual("Document update conflict.", db.ServerResponse.Reason);
        }

        [Test]
        public void Save_NullDocument_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Save(null));
        }

        [Test]
        public void Save_DocumentNoRevision_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<InvalidOperationException>(() => db.Save(new ExampleEntity { Id = "some_doc_id", Age = 22, IsAlive = false, Name = "Bob" }));
        }

        [Test]
        public void Delete_NullValues_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Delete(null, null));

        }

        [Test]
        public void Delete_NullValue_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Delete(null));
        }

        [Test]
        public void Delete_NameRevision_Deleted()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Delete("unittest/some_doc_id?rev=1234")).Returns(_deleteDocumentResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.Delete("some_doc_id","1234");

            Assert.IsNotNull(db.ServerResponse);
            Assert.IsTrue(db.ServerResponse.Ok);
            Assert.AreEqual("2-1234",db.ServerResponse.Revision);
        }

        [Test]
        public void Delete_DocumentObject_Deleted()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Delete("unittest/some_doc_id?rev=1234")).Returns(_deleteDocumentResponse.Object);

            var docMock = new Mock<ICouchDocument>(MockBehavior.Strict);
            docMock.Setup(s => s.Id).Returns("some_doc_id");
            docMock.Setup(s => s.Revision).Returns("1234");

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.Delete(docMock.Object);

            Assert.IsNotNull(db.ServerResponse);
            Assert.IsTrue(db.ServerResponse.Ok);
            Assert.AreEqual("2-1234", db.ServerResponse.Revision);
        }

        [Test]
        public void Delete_NoRevision_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var docMock = new Mock<ICouchDocument>(MockBehavior.Strict);
            docMock.SetupAllProperties();

            var db = new CouchDatabase(_connectionMock.Object, "unittest");

            Assert.Throws<InvalidOperationException>(() => db.Delete(docMock.Object));
        }

        [Test]
        public void Delete_Error_HandlesError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Delete("unittest/some_doc_id?rev=1234")).Returns(_deleteDocumentErrorResponse.Object);

            var docMock = new Mock<ICouchDocument>(MockBehavior.Strict);
            docMock.Setup(s => s.Id).Returns("some_doc_id");
            docMock.Setup(s => s.Revision).Returns("1234");

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.Delete(docMock.Object);

            Assert.IsNotNull(db.ServerResponse);
            Assert.IsFalse(db.ServerResponse.Ok);    
        }

        [Test]
        public void GetAll_Get_CanParse()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs")).Returns(_getAllIdsResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll().ToList();

            Assert.NotNull(results);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("4c51bc81501dd2ee3d20e981a8000562", results[0].Id);
            Assert.AreEqual("1-0aaca901d8459d637c4cdc143dc49f65", results[0].Revision);
        }

        [Test]
        public void GetAll_WithOptions_CanParse()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs?limit=10&startkey=%22test1%22&endkey=%22test52%22&descending=true")).Returns(_getAllIdsResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll(10, "test1", "test52", true);

            Assert.NotNull(results);
            Assert.AreEqual(2, results.Count());
        }

        [Test]
        public void GetAll_Error_CanHandle()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs")).Returns(_getAllIdsErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll();

            Assert.NotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public void GetAllObj_Get_CanParse()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs?include_docs=true")).Returns(_getAllObjResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll<ExampleEntity>();

            Assert.NotNull(results);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Fred Smith", results.ToList()[0].Name);
            Assert.AreEqual("Bill Smith", results.ToList()[1].Name);
        }

        [Test]
        public void GetAllObj_WithOptions_CanParse()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs?limit=10&startkey=%22test1%22&endkey=%22test52%22&descending=true&include_docs=true")).Returns(_getAllObjResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll<ExampleEntity>(10, "test1", "test52", true);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Fred Smith", results.ToList()[0].Name);
            Assert.AreEqual("Bill Smith", results.ToList()[1].Name);
        }

        [Test]
        public void GetAllObj_Error_CanHandle()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs?include_docs=true")).Returns(_getAllIdsErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll<ExampleEntity>();

            Assert.NotNull(results);
            Assert.AreEqual(0, results.Count());
        }
    }
}