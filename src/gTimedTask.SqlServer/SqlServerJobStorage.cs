using gTimedTask.Core.Storage;
using System;
using System.Data;
using System.Data.SqlClient;
namespace gTimedTask.SqlServer
{
    public class SqlServerJobStorage : IJobStorageConnection
    {
        private string _connectionString;
        public SqlServerJobStorage(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IDbConnection GetDbConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
