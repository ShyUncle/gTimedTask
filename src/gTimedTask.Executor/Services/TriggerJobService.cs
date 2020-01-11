using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
namespace gTimedTask.Executor
{
    public class TriggerJobService : JobHandlerTrigger.JobHandlerTriggerBase
    {
        private readonly ILogger<TriggerJobService> _logger;
        private ExecutorManager _ExecutorManager;
        public TriggerJobService(ILogger<TriggerJobService> logger, ExecutorManager executorManager)
        {
            _logger = logger;
            this._ExecutorManager = executorManager;
        }
        private static List<string> li = new List<string>();

        public override Task<JobReply> TriggerJob(JobHandlerTriggerRequest request, ServerCallContext context)
        {
            _ExecutorManager.Trigger(request.Name);
            return Task.FromResult(new JobReply() { Message = "" });
        }

        public override Task<JobReplys> GetJobHandler(JobHandlersRequest request, ServerCallContext context)
        {
            var aad = new JobReplys();
            aad.Anames.AddRange(_ExecutorManager.jobHandlerRepository.Keys.ToList());
            return Task.FromResult(aad);
        }
        //public override TriggerJobService Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        //{
        //    li.Add(request.Name);
        //    return Task.FromResult(new HelloReply
        //    {
        //        Message = $"Hello {request.Name}"
        //    });
        //}


    }
}
