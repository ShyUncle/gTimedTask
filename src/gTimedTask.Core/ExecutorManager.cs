using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gTimedTask.Core
{
    /// <summary>
    /// 执行器管理
    /// </summary>
    public class ExecutorManager
    {
        public static Dictionary<string, Dictionary<string, string>> executor = new Dictionary<string, Dictionary<string, string>>();
        public static void Register(string executorName, string handlerName, string address)
        {
            if (!executor.ContainsKey(handlerName))
            {
                executor.Add(handlerName, new Dictionary<string, string>());
            }
            if (!executor[handlerName].ContainsKey(executorName))
            {
                executor[handlerName].Add(executorName, address);
            }
        }

        public static string Get(string handlerName)
        {
            return executor[executor.Keys.Where(x => x.Equals(handlerName)).FirstOrDefault()].Values.FirstOrDefault();
        }
    }
}
