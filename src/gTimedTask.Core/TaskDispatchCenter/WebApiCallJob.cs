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
    /// httpclient调用Api接口
    /// </summary>
    //[Quartz.DisallowConcurrentExecution]
    public class WebApiCallJob : IJob
    {
        private IHttpClientFactory _httpClientFactory;
        //public RemoteHttpJob(System.Net.Http.IHttpClientFactory clientFactory)
        //{
        //    this._httpClientFactory = clientFactory;
        //}
        public async Task Execute(IJobExecutionContext context)
        {
            var s = context.JobDetail;

            var url = s.JobDataMap.Get("address");
         
            JobKey jobKey = context.Trigger.JobKey;
            Console.WriteLine(DateTime.Now);
            // trigger
            //  JobTrriger.Trigger(jobId);
        }
    }
}
