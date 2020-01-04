using System;
using System.Data;
using gTimedTask.Core.Storage;
using Microsoft.Data.Sqlite;

namespace gTimedTask.SQLite
{
    public class SQLiteJobStorage : IJobStorageConnection
    {
        private string _connectionString;
        public SQLiteJobStorage(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IDbConnection GetDbConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
