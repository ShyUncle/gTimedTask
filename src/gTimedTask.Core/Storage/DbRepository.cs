using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
namespace gTimedTask.Core.Storage
{
    public class DbRepository
    {
        private DbConnectionFactory _connectionFactory;
        public DbRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        private IDbConnection GetConnection()
        {
            return _connectionFactory.CreateConnection();
        }

        #region 异步查询

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            using (var con = GetConnection())
            {
                var result = await con.QueryAsync<T>(sql, param);
                return result;
            }
        }
        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            using (var con = GetConnection())
            {
                var result = await con.GetAllAsync<T>();
                return result;
            }
        }

        public async Task<T> GetAsync<T>(long id) where T : class
        {
            using (var con = GetConnection())
            {
                var result = await con.GetAsync<T>(id);
                return result;
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
        {
            using (var con = GetConnection())
            {
                var result = await con.QueryFirstOrDefaultAsync<T>(sql, param);
                return result;
            }
        }

        public async Task<bool> UpdateAsync<T>(T entity) where T : class
        {
            using (var con = GetConnection())
            {
                var result = await con.UpdateAsync(entity);
                return result;
            }
        }

        public async Task<int> InsertAsync<T>(T entity) where T : class
        {
            using (var con = GetConnection())
            {
                try
                {
                    var result = await con.InsertAsync(entity);
                    return result;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public async Task<bool> DeleteAsync<T>(T entity) where T : class
        {
            using (var con = GetConnection())
            {
                var result = await con.DeleteAsync(entity);
                return result;
            }
        }

        #endregion

    }
}
