using System;

namespace gTimedTask.Executor
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
