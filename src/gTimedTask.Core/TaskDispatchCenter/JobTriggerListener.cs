using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gTimedTask.Core.TaskDispatchCenter
{
    public class JobTriggerListener : ITriggerListener
    {
        public string Name => "global";

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("执行完成");
           
            return Task.CompletedTask;
            // throw new NotImplementedException();
        }

        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("执行中");
            return Task.CompletedTask;
            // throw new NotImplementedException();
        }

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            Console.WriteLine("错过执行");
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("执行是否不执行");
            return Task.FromResult(false);
            //  throw new NotImplementedException();
        }
    }
}
