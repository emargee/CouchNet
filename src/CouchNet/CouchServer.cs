using System;

namespace CouchNet
{
    public class CouchServer
    {
        private ICouchConnection _connection;

        #region ctor

        public CouchServer(ICouchConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException();
            }

            _connection = connection;
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
    }
}