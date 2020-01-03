using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace gTimedTask.Executor
{
    public static class ExecutorExtension
    {
        public static IServiceCollection AddExecutor(this IServiceCollection services)
        {
            services.AddSingleton<ExecutorManager>();
            return services;
        }
        public static IApplicationBuilder UseExecutor(this IApplicationBuilder app)
        {
            var executorManager = app.ApplicationServices.GetService<ExecutorManager>();
            executorManager.ExecutorRegister(null);
            return app;
        }

        public static IApplicationBuilder UseExecutor(this IApplicationBuilder app, Action<JobExecutorOption> optionConfig)
        {
            var executorManager = app.ApplicationServices.GetService<ExecutorManager>();
            var jobOption = new JobExecutorOption();
            optionConfig.Invoke(jobOption);
            executorManager.ExecutorRegister(jobOption);
            return app;
        }
    }
}
