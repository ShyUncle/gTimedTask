using gTimedTask.Core;
using gTimedTask.Core.Api;
using gTimedTask.Core.Storage;
using gTimedTask.RegistrationCenter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace gTimedTask
{
    public static class MiddlewareExtension
    {
        public static IServiceCollection AddgTimedTask(this IServiceCollection services, Action<gTimedTaskOptions> config)
        {
            //   services.AddSingleton<ApiMiddleware>();
            services.AddTransient<DbRepository>();
            services.AddTransient<IJobService, JobService>();
            services.AddSingleton<ApiDispatcher>().AddSingleton<DbConnectionFactory>();
            //  services.AddSingleton<UIMiddleware>();
            gTimedTaskOptions options = new gTimedTaskOptions();
            config.Invoke(options);
            services.AddSingleton(typeof(IJobStorageConnection), options.jobStorageConnection);
            return services;
        }
        public static IApplicationBuilder UsegTimedTask(this IApplicationBuilder app)
        {
            var hostApplicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                 
            });
            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                DynamicJobScheduler.Start();
                JobExecutorManager.HealthCheck();
                Console.WriteLine("服务主机启动成功，可以启动其他任务");
            });
            app.UseMiddleware<ApiMiddleware>();
            return app;
        }
        public static IApplicationBuilder UsegTimedTaskUI(this IApplicationBuilder app)
        {
            app.UseMiddleware<UIMiddleware>();
            return app;
        }
    }

    public class gTimedTaskOptions
    {
        public IJobStorageConnection jobStorageConnection { get; set; }
    }
}
