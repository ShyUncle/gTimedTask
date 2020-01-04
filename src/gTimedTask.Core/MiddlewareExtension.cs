using gTimedTask;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace gTimedTask
{
    public static class MiddlewareExtension
    {
        public static IServiceCollection AddgTimedTask(this IServiceCollection services)
        {
            services.AddSingleton<ApiMiddleware>();
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
