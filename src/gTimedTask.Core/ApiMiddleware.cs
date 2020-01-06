using gTimedTask.RegistrationCenter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gTimedTask
{
    public class ApiMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var tm = new TemplateMatcher(TemplateParser.Parse("gTimedTaskapi/default"), new RouteValueDictionary());
            if (tm.TryMatch(context.Request.Path, context.Request.RouteValues))
            {
                string method = "";
                if (context.Request.Method == HttpMethods.Post)
                {
                    var req = context.Request;
                    req.EnableBuffering();


                    using (var reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                    {
                        var a = await reader.ReadToEndAsync();
                        var job = System.Text.Json.JsonSerializer.Deserialize<JobExecutor>(a);
                        JobExecutorManager.Register(job);
                    }

                    // 这里读取过body  Position是读取过几次  而此操作优于控制器先行 控制器只会读取Position为零次的

                    req.Body.Position = 0;
                    Console.WriteLine("post请求" + System.Text.Json.JsonSerializer.Serialize(context.Request.RouteValues));
                }
            }
            await next(context);
            return;
        }
    }
    public class UIMiddleware
    {
        public string RoutePrefix { get; set; } = "gTimedTask";
        private StaticFileMiddleware _staticFileMiddleware;
        private readonly RequestDelegate _nextDelegate;
        public UIMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnv, ILoggerFactory loggerFactory)
        {
            _nextDelegate = next;
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = $"/{RoutePrefix}",
                FileProvider = new EmbeddedFileProvider(typeof(UIMiddleware).Assembly, "gTimedTask.Core.html"),
            };

            _staticFileMiddleware = new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.ToString().Contains(RoutePrefix))
            {
                if (Regex.IsMatch(context.Request.Path.Value, $"^/{RoutePrefix}/?$"))
                {
                    // Use relative redirect to support proxy environments
                    var relativeRedirectPath = context.Request.Path.Value.EndsWith("/")
                        ? "index.html"
                        : $"{ context.Request.Path.Value.Split('/').Last()}/index.html";
                    context.Response.StatusCode = 301;
                    context.Response.Headers["Location"] = relativeRedirectPath;
                    return;
                }
                if (Regex.IsMatch(context.Request.Path.Value, $"/{RoutePrefix}/?index.html"))
                {
                    await RespondWithIndexHtml(context.Response);
                    return;
                }
            }

            await _staticFileMiddleware.Invoke(context);
        }
        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";
            var s = typeof(UIMiddleware).Assembly
            .GetManifestResourceStream("gTimedTask.Core.html.index.html");
            using (var stream = s)
            {
                // Inject arguments before writing to response
                var htmlBuilder = new StringBuilder(new StreamReader(stream).ReadToEnd());
                //foreach (var entry in GetIndexArguments())
                //{
                //    htmlBuilder.Replace(entry.Key, entry.Value);
                //}
                await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
            }
        }
    }
}
