using gTimedTask.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gTimedTask.Executor.Handler
{
    public class LogHandler : IJobHandler
    {
        public object Execute(string param)
        {
            Console.WriteLine("执行了" + DateTime.Now);
            Thread.Sleep(1000);
            return null;
        }
    }
}
