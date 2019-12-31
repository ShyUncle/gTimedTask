using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gTimedTask.Core.RegistrationCenter
{
    public class JobExecutorHealth
    {
        public void HealthCheck() { 
        
        }
    }

    public enum ExecutorStatus
    {
        Unknown = 0,
        Serving = 1,
        NotServing = 2
    }
}
