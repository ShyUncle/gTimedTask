using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace gTimedTask.Core.Storage
{
    public class DbConnectionFactory
    {
        private IJobStorageConnection _jobStorage;
        public DbConnectionFactory(IJobStorageConnection jobStorage)
        {
            _jobStorage = jobStorage;
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection()
        {
            var conn = _jobStorage.GetDbConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }
    }
}
