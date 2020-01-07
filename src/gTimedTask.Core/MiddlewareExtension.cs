using gTimedTask.Core;
using gTimedTask.Core.Api;
using gTimedTask.Core.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
namespace gTimedTask
{
    public static class MiddlewareExtension
    {
        public static IServiceCollection AddgTimedTask(this IServiceCollection services)
        {
         //   services.AddSingleton<ApiMiddleware>();
            services.AddTransient<DbRepository>();
            services.AddTransient<IJobService, JobService>();
            services.AddSingleton<ApiDispatcher>().AddSingleton<DbConnectionFactory>();
            //  services.AddSingleton<UIMiddleware>();
            return services;
        }
        public static IApplicationBuilder UsegTimedTask(this IApplicationBuilder app)
        {
            DynamicJobScheduler.Start();

            app.UseMiddleware<ApiMiddleware>();
            return app;
        }
        public static IApplicationBuilder UsegTimedTaskUI(this IApplicationBuilder app)
        {
            app.UseMiddleware<UIMiddleware>();
            return app;
        }
    }
}
