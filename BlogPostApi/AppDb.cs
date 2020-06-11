using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace BlogPostApi
{
    public class AppDb:IDisposable
    {
        public MySqlConnection Connection { get; }
        public AppDb(string connectionString) {
            Connection = new MySqlConnection(connectionString);
        }
        // Gets rid off this resource immediatelly
        public void Dispose() => Connection.Dispose();
    }
}
