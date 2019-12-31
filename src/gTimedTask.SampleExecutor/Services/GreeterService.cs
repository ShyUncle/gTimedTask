using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using gTimedTask.Core;
namespace gTimedTask.SampleExecutor
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private ExecutorManager _ExecutorManager;
        public GreeterService(ILogger<GreeterService> logger, ExecutorManager executorManager)
        {
            _logger = logger;
            this._ExecutorManager = executorManager;
        }
        private static List<string> li = new List<string>();
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            li.Add(request.Name);
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {request.Name}"
            });
        }

        public override Task<HelloReplys> GetHello(HelloRequest request, ServerCallContext context)
        {
            _ExecutorManager.Trigger(request.Name);
            return Task.FromResult(new HelloReplys());
        }
    }
}
