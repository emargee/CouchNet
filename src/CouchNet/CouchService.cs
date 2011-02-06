using System;
using System.Net;
using CouchNet.Enums;
using CouchNet.Exceptions;
using CouchNet.Impl;
using CouchNet.Impl.ServerResponse;
using Newtonsoft.Json;

namespace CouchNet
{
    public class CouchService
    {
        internal ICouchConnection Connection { get; set; }

        internal static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };

        public bool EnableValidation { get; set; }

        public CouchDatabase this[string name]
        {
            get
            {
                return GetDatabase(name);
            }
        }

        #region ctor

        public CouchService(ICouchConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException();
            }

            Connection = connection;
        }

        public CouchService(string connectionString)
        {
            Connection = new CouchConnection(connectionString);
        }

        #endregion

        #region Database Control

        public CouchDatabase CreateDatabase(string name)
        {
            throw new NotImplementedException();
        }

        public ICouchServerResponse DropDatabase(string name)
        {
            throw new NotImplementedException();
        }

        public CouchDatabase GetDatabase(string name)
        {
            return new CouchDatabase(name, this);
        }

        private bool DatabaseExists(CouchDatabase database)
        {
            return database != null && DatabaseExists(database.Name);
        }

        private bool DatabaseExists(string databaseName)
        {
            var head = Connection.Head(databaseName).StatusCode;

            return head == HttpStatusCode.OK || head == HttpStatusCode.NotModified;
        }

        #endregion

        #region Session Control

        public void GetSessionInfo()
        {
            //Not logged in : {"ok":true,"userCtx":{"name":null,"roles":[]},"info":{"authentication_db":"_users","authentication_handlers":["oauth","cookie","default"]}}
            //Logged in     : {"ok":true,"userCtx":{"name":"emargee","roles":["_admin"]},"info":{"authentication_db":"_users","authentication_handlers":["oauth","cookie","default"],"authenticated":"cookie"}}
            throw new NotImplementedException();
        }

        public void SessionLogin(string username, string password)
        {
            //POST name=emargee&password=blah
            //Response header : Set-Cookie: AuthSession=ZW1hcmdlZTo0QzczMDBGNjrQIpYB_QfehSE3zZxgvB3hHBIHEw; Version=1; Path=/; HttpOnly
        }

        public void SessionLogout()
        {
            //DELETE /_session
        }

        #endregion

        #region Stats

        //http://wiki.apache.org/couchdb/Runtime_Statistics
        //_stats

        #endregion

        #region Config

        public void QueryServers()
        {
            //_config/query_servers
        }

        #endregion

        #region Maintenance

        public ICouchServerResponse BeginDesignDocumentCompact(CouchDesignDocument designDocument)
        {
            if (designDocument == null)
            {
                throw new ArgumentNullException("designDocument");
            }

            if (EnableValidation)
            {
                var head = Connection.Head(designDocument.Database.Name + "/" + designDocument.Id).StatusCode;

                if (head != HttpStatusCode.OK && head != HttpStatusCode.NotModified)
                {
                    throw new CouchDocumentNotFoundException(designDocument.Name);
                }
            }

            var path = string.Format("{0}/_compact/{1}", designDocument.Database.Name, designDocument.Id);
            var response = Connection.Post(path, null);

            return new CouchServerResponse(response);
        }

        public ICouchServerResponse BeginDatabaseCompact(CouchDatabase database)
        {
            if(database == null)
            {
                throw new ArgumentNullException("database");
            }

            if (EnableValidation)
            {
                if (!DatabaseExists(database))
                {
                    throw new CouchDatabaseNotFoundException(database.Name);
                }
            }

            var path = string.Format("{0}/_compact", database.Name);
            var response = Connection.Post(path, null);

            return new CouchServerResponse(response);
        }

        public ICouchServerResponse BeginDatabaseViewCleanup(CouchDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            if (EnableValidation)
            {
                if(!DatabaseExists(database))
                {
                    throw new CouchDatabaseNotFoundException(database.Name);
                }
            }

            var path = string.Format("{0}/_view_cleanup", database.Name);
            var response = Connection.Post(path, null);

            return new CouchServerResponse(response);
        }

        #endregion

        #region Info

        public CouchDesignDocumentInfoResponse DesignDocumentInfo(CouchDesignDocument designDocument)
        {
            if (designDocument == null)
            {
                throw new ArgumentNullException("designDocument");
            }

            if (EnableValidation)
            {
                var head = Connection.Head(designDocument.Database.Name + "/" + designDocument.Id).StatusCode;

                if (head != HttpStatusCode.OK && head != HttpStatusCode.NotModified)
                {
                    throw new CouchDocumentNotFoundException(designDocument.Name);
                }
            }

            var path = string.Format("{0}/{1}/_info", designDocument.Database.Name, designDocument.Id);

            var rawResponse = Connection.Get(path);

            return new CouchDesignDocumentInfoResponse(rawResponse, JsonSettings);
        }

        #endregion
    }
}