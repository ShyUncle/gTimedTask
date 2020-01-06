using Grpc.Net.Client;
using gTimedTask.RegistrationCenter;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace gTimedTask
{
    /// <summary>
    /// 定时任务中心
    /// </summary>
    public class DynamicJobScheduler
    {
        static IScheduler scheduler = null;
        public static async void Start()
        {
            //开启socket服务端
            //    JobServer.Start();
            //开启定时任务
            NameValueCollection properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "GCScheduler";
            properties["quartz.scheduler.instanceId"] = "AUTO";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "20";
            properties["quartz.threadPool.threadPriority"] = "Normal";
            properties["quartz.jobStore.misfireThreshold"] = "60000";
            properties["quartz.jobStore.useProperties"] = "false";
            properties["quartz.jobStore.clustered"] = "false";// "true";是否集群
            properties["quartz.serializer.type"] = "binary";
            //存储类型
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            //表明前缀
            properties["quartz.jobStore.tablePrefix"] = "GTIMEDTASK_";
            //驱动类型
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz";
            //数据源名称
            properties["quartz.jobStore.dataSource"] = "myDS";
            //连接字符串
            string path = AppDomain.CurrentDomain.BaseDirectory + "Db/gTimedTask.db";
            properties["quartz.dataSource.myDS.connectionString"] = $"Data Source={path}";
            //mysqlserver版本
            properties["quartz.dataSource.myDS.provider"] = "SQLite-Microsoft";
            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            scheduler = await sf.GetScheduler();
            await scheduler.Start();

            scheduler.ListenerManager.AddTriggerListener(new TriggerListener());
            //  JobMonitorHelper.Start();
        }

        ///// <summary>
        ///// 填充状态
        ///// </summary>
        ///// <param name="jobInfo"></param>
        //public static async void FillJobInfo(GCJobInfo jobInfo)
        //{
        //    // TriggerKey : name + group
        //    TriggerKey triggerKey = new TriggerKey(jobInfo.Id.ToString(), jobInfo.JobExecutorKey.ToString());

        //    try
        //    {
        //        TriggerState triggerState = await scheduler.GetTriggerState(triggerKey);
        //        jobInfo.JobStatus = triggerState.ToString();
        //    }
        //    catch (SchedulerException e)
        //    {
        //    }
        //}

        /// <summary>
        /// 查看任务是否存在
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <returns></returns>
        public static async Task<bool> CheckExists(string jobName, string jobGroup)
        {
            TriggerKey triggerKey = new TriggerKey(jobName, jobGroup);
            return await scheduler.CheckExists(triggerKey);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        public static async Task<bool> AddJob(string jobName, string jobGroup, string cronExpression)
        {
            // TriggerKey : name + group
            TriggerKey triggerKey = new TriggerKey(jobName, jobGroup);
            JobKey jobKey = new JobKey(jobName, jobGroup);

            // TriggerKey valid if_exists
            if (await CheckExists(jobName, jobGroup))
            {
                // job  exists, JobGroup:{}, JobName:{}", jobGroup, jobName);
                return false;
            }

            // CronTrigger : TriggerKey + cronExpression	// withMisfireHandlingInstructionDoNothing 忽略掉调度终止过程中忽略的调度
            CronScheduleBuilder cronScheduleBuilder = CronScheduleBuilder.CronSchedule(cronExpression).WithMisfireHandlingInstructionDoNothing();
            var cronTrigger = TriggerBuilder.Create().WithIdentity(triggerKey).WithSchedule(cronScheduleBuilder).Build();

            // JobDetail 
            IJobDetail jobDetail = JobBuilder.Create(typeof(RemoteHttpJob)).WithIdentity(jobKey).WithDescription("测试").UsingJobData("dd", "1").Build();
            // schedule : jobDetail + cronTrigger
            await scheduler.ScheduleJob(jobDetail, cronTrigger);
            return true;
        }

        /**
         * rescheduleJob
         *
         * @param jobGroup
         * @param jobName
         * @param cronExpression
         * @return
         * @throws SchedulerException
         */
        public static async Task<bool> RescheduleJob(string jobName, string jobGroup, string cronExpression)
        {

            // TriggerKey valid if_exists
            if (!await CheckExists(jobName, jobGroup))
            {
                //">>>>>>>>>>> rescheduleJob fail, job not exists, JobGroup:{}, JobName:{}", jobGroup, jobName);
                return false;
            }

            // TriggerKey : name + group
            TriggerKey triggerKey = new TriggerKey(jobName, jobGroup);
            var oldTrigger = await scheduler.GetTrigger(triggerKey) as CronTriggerImpl;

            if (oldTrigger != null)
            {
                // avoid repeat
                string oldCron = oldTrigger.GetExpressionSummary();
                if (oldCron.Equals(cronExpression))
                {
                    return true;
                }

                // CronTrigger : TriggerKey + cronExpression
                CronScheduleBuilder cronScheduleBuilder = CronScheduleBuilder.CronSchedule(cronExpression).WithMisfireHandlingInstructionDoNothing();
                oldTrigger = oldTrigger.GetTriggerBuilder().WithIdentity(triggerKey).WithSchedule(cronScheduleBuilder).Build() as CronTriggerImpl;

                // rescheduleJob
                await scheduler.RescheduleJob(triggerKey, oldTrigger);
            }
            else
            {
                // CronTrigger : TriggerKey + cronExpression
                CronScheduleBuilder cronScheduleBuilder = CronScheduleBuilder.CronSchedule(cronExpression).WithMisfireHandlingInstructionDoNothing();
                var cronTrigger = TriggerBuilder.Create().WithIdentity(triggerKey).WithSchedule(cronScheduleBuilder).Build();

                // JobDetail-JobDataMap fresh
                JobKey jobKey = new JobKey(jobName, jobGroup);
                var jobDetail = await scheduler.GetJobDetail(jobKey);



                await scheduler.ScheduleJob(jobDetail, cronTrigger);
            }
            //await new gTimedTask.Core.Storage.DbRepository(null).InsertAsync<gTimedTask.Core.DomainModel.JobEntity>(new Core.DomainModel.JobEntity()
            //{
            //    CreateTime = DateTime.Now.Ticks,
            //    Cron = cronExpression,
            //     Description=""
            //}); ;
            //logger.info(">>>>>>>>>>> resumeJob success, JobGroup:{}, JobName:{}", jobGroup, jobName);
            return true;
        }

        /**
         * unscheduleJob
         *
         * @param jobName
         * @param jobGroup
         * @return
         * @throws SchedulerException
         */
        public static async Task<bool> RemoveJob(string jobName, string jobGroup)
        {
            // TriggerKey : name + group
            TriggerKey triggerKey = new TriggerKey(jobName, jobGroup);
            bool result = false;
            if (await CheckExists(jobName, jobGroup))
            {
                result = await scheduler.UnscheduleJob(triggerKey);
                // logger.info(">>>>>>>>>>> removeJob, triggerKey:{}, result [{}]", triggerKey, result);
            }
            return result;
        }

        /**
         * pause
         *
         * @param jobName
         * @param jobGroup
         * @return
         * @throws SchedulerException
         */
        public static async Task<bool> PauseJob(string jobName, string jobGroup)
        {
            // TriggerKey : name + group
            TriggerKey triggerKey = new TriggerKey(jobName, jobGroup);
            bool result = false;
            if (await CheckExists(jobName, jobGroup))
            {
                await scheduler.PauseTrigger(triggerKey);
                result = true;
                //  logger.info(">>>>>>>>>>> pauseJob success, triggerKey:{}", triggerKey);
            }
            else
            {
                //  logger.info(">>>>>>>>>>> pauseJob fail, triggerKey:{}", triggerKey);
            }
            return result;
        }

        /**
         * resume
         *
         * @param jobName
         * @param jobGroup
         * @return
         * @throws SchedulerException
         */
        public static async Task<bool> ResumeJob(string jobName, string jobGroup)
        {
            TriggerKey triggerKey = new TriggerKey(jobName, jobGroup);
            bool result = false;
            if (await CheckExists(jobName, jobGroup))
            {
                await scheduler.ResumeTrigger(triggerKey);
                result = true;
                // logger.info(">>>>>>>>>>> resumeJob success, triggerKey:{}", triggerKey);
            }
            else
            {
                //   logger.info(">>>>>>>>>>> resumeJob fail, triggerKey:{}", triggerKey);
            }
            return result;
        }

        /**
         * run
         *
         * @param jobName
         * @param jobGroup
         * @return
         * @throws SchedulerException
         */
        public static async Task<bool> TriggerJob(string jobName, string jobGroup)
        {
            // TriggerKey : name + group
            JobKey jobKey = new JobKey(jobName, jobGroup);
            bool result = false;
            if (await CheckExists(jobName, jobGroup))
            {
                await scheduler.TriggerJob(jobKey);
                result = true;
                //  logger.info(">>>>>>>>>>> runJob success, jobKey:{}", jobKey);
            }
            else
            {
                //logger.info(">>>>>>>>>>> runJob fail, jobKey:{}", jobKey);
            }
            return result;
        }

        public static async Task<object> GetList()
        {
            // var list= await scheduler.GetTriggerState();
            return null;
        }


    }

    public class TriggerListener : ITriggerListener
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
    [Quartz.DisallowConcurrentExecution]
    public class RemoteHttpJob : IJob
    {
        private IHttpClientFactory _httpClientFactory;
        //public RemoteHttpJob(System.Net.Http.IHttpClientFactory clientFactory)
        //{
        //    this._httpClientFactory = clientFactory;
        //}
        public async Task Execute(IJobExecutionContext context)
        {
            var s = context.JobDetail;

            //   var url = s.JobDataMap.Get("url").ToString();
            var executor = JobExecutorManager.GetExecutor(s.Key.Name, LoadBalanceStrategy.First);
            if (executor == null)
            {
                return;
            }
            var address = executor.Address;
            var channel = GrpcChannel.ForAddress(address);

            var greeterClient = new Greeter.GreeterClient(channel);
            await greeterClient.GetHelloAsync(new HelloRequest { Name = s.Key.Name });
            context.Result = "a";
            JobKey jobKey = context.Trigger.JobKey;
            Console.WriteLine(DateTime.Now);
            // trigger
            //  JobTrriger.Trigger(jobId);
        }
    }
}