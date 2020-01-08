using Grpc.Net.Client;
using gTimedTask.RegistrationCenter;
using Quartz;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace gTimedTask.Core
{
    /// <summary>
    /// 远程调用执行器任务
    /// </summary>
    //[Quartz.DisallowConcurrentExecution]
    public class RemoteCallJob : IJob
    {
        private IHttpClientFactory _httpClientFactory;
        //public RemoteHttpJob(System.Net.Http.IHttpClientFactory clientFactory)
        //{
        //    this._httpClientFactory = clientFactory;
        //}

        public static Dictionary<string, Greeter.GreeterClient> dicClient = new Dictionary<string, Greeter.GreeterClient>();
        public async Task Execute(IJobExecutionContext context)
        {
            var s = context.JobDetail;

            var url = s.JobDataMap.Get("url");
            var executor = JobExecutorManager.GetExecutor("gTimedTask.Executor.Handler.LogHandler", LoadBalanceStrategy.First);//s.Key.Name, LoadBalanceStrategy.First);
            if (executor == null)
            {
                return;
            }
            var address = executor.Address;
            Greeter.GreeterClient greeterClient = null;
            if (dicClient.ContainsKey(address))
            {
                greeterClient = dicClient[address];
            }
            else
            {
                GrpcChannel channel = GrpcChannel.ForAddress(address);
                greeterClient = new Greeter.GreeterClient(channel); 
                dicClient[address] = greeterClient;
            }

            await greeterClient.GetHelloAsync(new HelloRequest { Name = s.Key.Name });
            context.Result = "a";
            JobKey jobKey = context.Trigger.JobKey;
            Console.WriteLine(DateTime.Now);
            // trigger
            //  JobTrriger.Trigger(jobId);
        }
    }
}
