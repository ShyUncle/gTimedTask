using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using Dapper.Contrib.Extensions;

namespace gTimedTask.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        public async Task<string> Index()
        {
            string aaa = AppContext.BaseDirectory;
            string path =AppDomain.CurrentDomain.BaseDirectory+ "wwwroot/gTimedTask.db";
            using (IDbConnection con = new SqliteConnection($"Data Source={path}"))
            {
                var a = await con.GetAsync<Test>(1);
                return a.Id.ToString();
            }
        }
    }

    [Table("test")]
    public class Test
    {
        public int Id { get; set; }
    }
}