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
using gTimedTask.Core.RegistrationCenter;

namespace gTimedTask.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {  

        [HttpGet]
        public async Task<string> Index()
        {
            await DynamicJobScheduler.AddJob("gTimedTask.SampleExecutor.Handler.LogHandler", "Test.Job1", "0/30 * * * * ?");
            //string aaa = AppContext.BaseDirectory;
            //string path = AppDomain.CurrentDomain.BaseDirectory + "Db/gTimedTask.db";
            //using (IDbConnection con = new SqliteConnection($"Data Source={path}"))
            //{
            //    var a = await con.GetAsync<Test>(1);
            //    return a.Id.ToString();
            //}
            return "33";
        }

        [HttpPost]
        public async Task<int> ServiceRegister([FromBody]JobExecutor r = default)
        {
              JobExecutorManager.Register(r);
            return await Task.FromResult(3);
        }
    }

    [Table("test")]
    public class Test
    {
        public int Id { get; set; }
    }
}