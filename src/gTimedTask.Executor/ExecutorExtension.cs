using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace gTimedTask.Executor
{
    public static class ExecutorExtension
    {
        public static IServiceCollection AddgTimedTaskExecutor(this IServiceCollection services, Action<JobExecutorOption> optionConfig)
        {
            services.AddGrpc();
            services.AddSingleton((s) =>
            {
                var configuration = s.GetService<IConfiguration>();
                var optionConfig = configuration.GetSection("JobExecutor").Get<JobExecutorOption>();
                var executor = new ExecutorManager(optionConfig);
                return executor;
            });

            return services;
        }
        public static IApplicationBuilder UsegTimedTaskExecutor(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException("builder");
            }
            var executorManager = app.ApplicationServices.GetService<ExecutorManager>();
            var hostApplicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                executorManager.ExecutorUnRegister();
            });
            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {

                executorManager.ExecutorRegister();
                Console.WriteLine("服务主机启动成功，可以启动其他任务");
            });
            app.UseEndpoints(e =>
            {
                e.MapGrpcService<GreeterService>();
                e.MapGrpcService<Services.HealthCheckService>();
            });
            return app;
        }
    }
}
