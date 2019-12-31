using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gTimedTask.Core.RegistrationCenter
{
    /// <summary>
    /// 执行器
    /// </summary>
    public class JobExecutorManager
    {
        private ConcurrentDictionary<string, JobExecutor> _list = new ConcurrentDictionary<string, JobExecutor>();
        /// <summary>
        /// 注册
        /// </summary>
        public void Register(JobExecutor jobExecutor)
        {
            _list.AddOrUpdate(jobExecutor.AppId, jobExecutor, (key, oldJobExecutor) => { return jobExecutor; });

        }

        /// <summary>
        /// 取消注册
        /// </summary>
        public void UnRegister(string appId)
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
        public List<JobExecutor> GetAll()
        {
            return _list.Values.ToList();
        }

        /// <summary>
        /// 根据负载均衡策略和执行任务名获取执行器
        /// </summary>
        /// <param name="jobHandlerName"></param>
        /// <param name="loadBalanceStrategy"></param>
        /// <returns></returns>
        public JobExecutor GetExecutor(string jobHandlerName, LoadBalanceStrategy loadBalanceStrategy)
        {
            var list = _list.Values.Where(x => x.JobHandler.Contains(jobHandlerName)).ToList();
            if (list.Count > 1)//一个不需要走策略
            {
            }
            return list.FirstOrDefault();
        }
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
        public int Port { get; set; }
        public List<string> JobHandler { get; set; } = new List<string>();
    }

}
