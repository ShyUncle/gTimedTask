using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using gTimedTask.Core;
using gTimedTask.Core.RegistrationCenter;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace gTimedTask.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddGrpcClient<Greeter.GreeterClient>(c => c.Address = new Uri("https://localhost:10101"));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            DynamicJobScheduler.Start();
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = $"/h1tml",
                FileProvider = new EmbeddedFileProvider(System.Reflection.Assembly.Load("gTimedTask"), "gTimedTask.html"),
            };
            app.UseStaticFiles(staticFileOptions);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            app.Use(async (context, next) =>
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
                   else
                   {
                       if (context.Request.Method == HttpMethods.Get)
                       {
                           Console.WriteLine("get请求" + System.Text.Json.JsonSerializer.Serialize(context.Request.RouteValues));
                       }
                   }
                   await next();
                   return;
               }
               if (context.Request.Path.ToString().Contains("h1tml"))
               {
                   if (Regex.IsMatch(context.Request.Path.Value, $"^/h1tml/?$"))
                   {
                       // Use relative redirect to support proxy environments
                       var relativeRedirectPath = context.Request.Path.Value.EndsWith("/")
                           ? "index.html"
                           : $"{ context.Request.Path.Value.Split('/').Last()}/index.html";
                       context.Response.StatusCode = 301;
                       context.Response.Headers["Location"] = relativeRedirectPath;

                       return;
                   }
                   await RespondWithIndexHtml(context.Response);
                   return;
               }
               await next();
               return;
           });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";
            var s = System.Reflection.Assembly.Load("gTimedTask")   // typeof(gTimedTask).GetTypeInfo().Assembly
            .GetManifestResourceStream("gTimedTask.html.index1.html");
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
