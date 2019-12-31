using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gTimedTask.SampleExecutor.Handler
{
    public class LogHandler : IJobHandler
    {
        public object Execute(string param)
        {
            Console.WriteLine("执行了" + DateTime.Now);
            return null;
        }
    }
}
