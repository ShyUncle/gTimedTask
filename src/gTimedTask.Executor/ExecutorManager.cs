using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
namespace gTimedTask.Executor
{
    public class JobExecutor
    {
        public string Name { get; set; }
        public string AppId { get; set; }
        public string Address { get; set; }
        public List<string> JobHandler { get; set; } = new List<string>();
        public string RegisterUrl { get; set; }
    }

    public class JobExecutorOption
    {
        public string Name { get; set; }
        public string AppId { get; set; }
        public string Address { get; set; }
        public string RegisterUrl { get; set; }
    }

    public class ExecutorManager
    {
        public ExecutorManager(JobExecutorOption option)
        {
            jobExecutorOption = option;
        }
        private JobExecutorOption jobExecutorOption;
        private JobExecutor jobExecutor;
        public Dictionary<string, Type> jobHandlerRepository = new Dictionary<string, Type>();
        public void ExecutorRegister()
        {
            //todo:使用依赖注入查找
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IJobHandler)))).ToList();
            foreach (var type in types)
            {
                jobHandlerRepository.Add(type.FullName, type);
            }
            if (jobExecutorOption == null)
            {
                throw new ArgumentNullException("JobExecutorOption");
            }
            //todo:参数检查
            jobExecutor = new JobExecutor()
            {
                Address = jobExecutorOption.Address,
                AppId = jobExecutorOption.AppId,
                Name = jobExecutorOption.Name,
                JobHandler = jobHandlerRepository.Keys.ToList(),
                RegisterUrl = jobExecutorOption.RegisterUrl
            };

            var content = new StringContent(JsonSerializer.Serialize(jobExecutor), System.Text.Encoding.UTF8, "application/json");
            //todo:使用httpclientFactory
            var r = new HttpClient().PostAsync(jobExecutorOption.RegisterUrl, content).Result.Content.ReadAsStringAsync().Result;
        }

        public void ExecutorUnRegister()
        {
            var content = new StringContent(JsonSerializer.Serialize(new { appId = jobExecutor.AppId }), System.Text.Encoding.UTF8, "application/json");
            //todo:使用httpclientFactory
            var r = new HttpClient().PutAsync(jobExecutor.RegisterUrl, content).Result.Content.ReadAsStringAsync().Result;
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
