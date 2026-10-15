using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLUNES
{
    public static class DB
    {
        private static readonly string connectionString =
            $@"Data Source=RAPATUMADRE\SQLEXPRESS; 
               Initial Catalog=BDPOSS; 
               Integrated Security=true;";

        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}