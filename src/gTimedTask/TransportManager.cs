using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Health.V1;
using System.Threading;
using gTimedTask.Core.RegistrationCenter;

namespace gTimedTask.Core
{
    /// <summary>
    /// 底层通信管理
    /// </summary>
    public class TransportManager
    {
        public async Task<ExecutorStatus> HealthCheck(string address)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:10101");
            var client = new Health.HealthClient(channel);
            var call = await client.CheckAsync(new HealthCheckRequest { Service = "" });
            return Enum.Parse<ExecutorStatus>(call.Status.ToString());
        }
    }
}
