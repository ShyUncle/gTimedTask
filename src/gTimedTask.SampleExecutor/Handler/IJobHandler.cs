using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gTimedTask.SampleExecutor.Handler
{
    public interface IJobHandler
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="param"> </param>
        /// <returns></returns>
        object Execute(string param);
    }
}
