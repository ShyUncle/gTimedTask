using gTimedTask.RegistrationCenter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace gTimedTask.Core.Api
{
    public class ApiMiddleware
    {

        private readonly RequestDelegate _nextDelegate;
        public ApiMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _nextDelegate = next;
        }
        public async Task InvokeAsync(HttpContext context, ApiDispatcher apiDispatcher)
        {
            var result = await apiDispatcher.Dispatch(context);
            if (result)
            {
                return;
            }
            var tm = new TemplateMatcher(TemplateParser.Parse("gTimedTaskapi/default"), new RouteValueDictionary());
            if (tm.TryMatch(context.Request.Path, context.Request.RouteValues))
            {
                string method = "";
                if (context.Request.Method == HttpMethods.Post)
                {
                    var req = context.Request;
                    req.EnableBuffering();


                    using (var reader = new StreamReader(req.Body, Encoding.UTF8))
                    {
                        var a = await reader.ReadToEndAsync();
                        var job = System.Text.Json.JsonSerializer.Deserialize<JobExecutor>(a);
                        JobExecutorManager.Register(job);
                    }

                    // 这里读取过body  Position是读取过几次  而此操作优于控制器先行 控制器只会读取Position为零次的

                    //req.Body.Position = 0;
                    Console.WriteLine("post请求" + System.Text.Json.JsonSerializer.Serialize(context.Request.RouteValues));
                }
            }
            await _nextDelegate(context);
        }
    }

}
