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
        [HttpGet]
        public async Task<string> Index()
        {
            await JobDynamicScheduler.AddJob("gTimedTask.SampleExecutor.Handler.LogHandler", "Test.Job1", "0/30 * * * * ?");
            string aaa = AppContext.BaseDirectory;
            string path = AppDomain.CurrentDomain.BaseDirectory + "Db/gTimedTask.db";
            using (IDbConnection con = new SqliteConnection($"Data Source={path}"))
            {
                var a = await con.GetAsync<Test>(1);
                return a.Id.ToString();
            }
        }
        public class Rec
        {
            public string Name { get; set; }
            public string Address { get; set; }
        }
        [HttpPost]
        public async Task<int> ServiceRegister([FromBody]Rec r = default)
        {

            ExecutorManager.Register("test", r.Name, r.Address);
            return await Task.FromResult(3);
        }
    }

    [Table("test")]
    public class Test
    {
        public int Id { get; set; }
    }
}