using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DbProvider.Pomocna
{
    public class DatabaseManager
    {
        public static Func<DbConnection> GetConnection = () => new SqlConnection(ConnectionString);

        //public static DbConnection GetConnection2 (string connectionString)=> new SqlConnection(connectionString);

        public static string ConnectionString =>
                //var dbc = GetConnection(ConnectionString);
                //return ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
                "here-goes-connection-string";



    }
}
