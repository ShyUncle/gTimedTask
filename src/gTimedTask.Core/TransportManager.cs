using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Health.V1;
using System.Threading;
using gTimedTask.RegistrationCenter;

namespace gTimedTask
{
    /// <summary>
    /// 底层通信管理
    /// </summary>
    public class TransportManager
    {
        private static Dictionary<string, GrpcChannel> dicChannel = new Dictionary<string, GrpcChannel>();
        public async Task<ExecutorStatus> HealthCheck(string address)
        {
            var channel = GetOrAddChannel(address);
            var client = new Health.HealthClient(channel);
            var call = await client.CheckAsync(new HealthCheckRequest { Service = "" });
            return Enum.Parse<ExecutorStatus>(call.Status.ToString());
        }

        public static GrpcChannel GetOrAddChannel(string address)
        {
            GrpcChannel channel = null;
            if (dicChannel.ContainsKey(address))
            {
                channel = dicChannel[address];
            }
            else
            {
                channel = GrpcChannel.ForAddress(address);
                dicChannel[address] = channel;
            }
            return channel;
        }
    }
}
