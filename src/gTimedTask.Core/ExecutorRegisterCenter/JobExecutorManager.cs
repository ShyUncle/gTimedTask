using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gTimedTask.RegistrationCenter
{
    /// <summary>
    /// 执行器
    /// </summary>
    public class JobExecutorManager
    {
        private static ConcurrentDictionary<string, JobExecutor> _list = new ConcurrentDictionary<string, JobExecutor>();
        static JobExecutorManager()
        {
            HealthCheck();
        }


        public static void HealthCheck()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var list = _list.Values.ToList();
                    foreach (var item in list)
                    {
                        var status = await new TransportManager().HealthCheck(item.Address);
                        if (status == ExecutorStatus.NotServing)
                        {
                            JobExecutor tmp = null;
                            _list.TryRemove(item.AppId, out tmp);
                        }
                    }
                    await Task.Delay(1000);
                }
            });
        }
        /// <summary>
        /// 注册
        /// </summary>
        public static void Register(JobExecutor jobExecutor)
        {
            _list.AddOrUpdate(jobExecutor.AppId, jobExecutor, (key, oldJobExecutor) => { return jobExecutor; });

        }

        /// <summary>
        /// 取消注册
        /// </summary>
        public static void UnRegister(string appId)
        {
            JobExecutor temp = null;
            var success = _list.TryRemove(appId, out temp);
            if (!success)//重试一次
            {
                _list.TryRemove(appId, out temp);
            }
        }

        /// <summary>
        /// 获取所有执行器
        /// </summary>
        /// <returns></returns>
        public static List<JobExecutor> GetAll()
        {
            return _list.Values.ToList();
        }

        /// <summary>
        /// 根据负载均衡策略和执行任务名获取执行器
        /// </summary>
        /// <param name="jobHandlerName"></param>
        /// <param name="loadBalanceStrategy"></param>
        /// <returns></returns>
        public static JobExecutor GetExecutor(string jobHandlerName, LoadBalanceStrategy loadBalanceStrategy)
        {
            var list = _list.Values.Where(x => x.JobHandler.Contains(jobHandlerName)).ToList();
            if (list.Count > 1)//一个不需要走策略
            {
            }
            return list.FirstOrDefault();
        }
    }

    public enum ExecutorStatus
    {
        Unknown = 0,
        Serving = 1,
        NotServing = 2
    }

    public enum LoadBalanceStrategy
    {
        /// <summary>
        /// 随机
        /// </summary>
        Random = 1,
        /// <summary>
        /// 第一个
        /// </summary>
        First,
        /// <summary>
        /// 轮循
        /// </summary>
        RoundRobin,
        /// <summary>
        /// 加权随机
        /// </summary>
        WeightRandom,
        /// <summary>
        /// 最小连接数
        /// </summary>
        LeastConnection,
        /// <summary>
        /// 最快响应
        /// </summary>
        LeastResponseTime

    }

    public class JobExecutor
    {
        public string Name { get; set; }
        public string AppId { get; set; }
        public string Address { get; set; }
        public List<string> JobHandler { get; set; } = new List<string>();
    }

}
