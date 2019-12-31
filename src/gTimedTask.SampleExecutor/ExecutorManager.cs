using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using gTimedTask.SampleExecutor.Handler;

namespace gTimedTask.SampleExecutor
{
    public class ExecutorManager
    {
        public Dictionary<string, Type> jobHandlerRepository = new Dictionary<string, Type>();
        public void ServiceRegister()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(Handler.IJobHandler)))).ToList();
            foreach (var type in types)
            {
                jobHandlerRepository.Add(type.FullName, type);
            }
            var content = new StringContent("{\"name\":\"" + jobHandlerRepository.Keys.FirstOrDefault() + "\",\"address\":\"https://localhost:10101\"}",System.Text.Encoding.UTF8,"application/json");
           var r= new HttpClient().PostAsync("https://localhost:5003/api/default", content).Result.Content.ReadAsStringAsync().Result;
        }

        public void Trigger(string handlerName)
        {
            if (jobHandlerRepository.Keys.Contains(handlerName))
            {
                var obj = Activator.CreateInstance(jobHandlerRepository[handlerName]) as IJobHandler;
                obj.Execute("");
            }
        }

    }
}
