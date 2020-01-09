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
    public interface ITransporter
    {
        /// <summary>
        /// 健康检查
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task HealthCheck(string address);
        /// <summary>
        /// 执行器任务调用
        /// </summary>
        /// <returns></returns>
        Task CallJobHandler(JobExecutor jobExecutor);

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<List<string>> GetJobHandler(string address);
    }

    /// <summary>
    /// 底层通讯框架
    /// </summary>
    public class GrpcTransporter : ITransporter
    {  
        public Task CallJobHandler(JobExecutor jobExecutor)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetJobHandler(string address)
        {
            throw new NotImplementedException();
        }

        public Task HealthCheck(string address)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 底层通信管理
    /// </summary>
    public class TransportManager
    {
        //todo:抽象
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
