using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;

namespace gTimedTask.Core.Api
{
    public class ApiDispatcher
    {
        public ApiDispatcher()
        {
            string RoutePrefix = "gTimedTask";
            var a = typeof(IJobService).Assembly.GetTypes();
            foreach (var b in a)
            {
                var me = b.GetMethods().Where(x => x.GetCustomAttributes(true).Any(x => x is gTimedTaskRouteAttribute));
                foreach (var item in me)
                {
                    var bd = item.GetCustomAttribute<gTimedTaskRouteAttribute>();

                    _templates.Add(new gTimedTaskTemplateMatcher()
                    {
                        ApiHttpMethod = bd.ApiHttpMethod,
                        ApiTargetType = b,
                        Name = item.Name,
                        RouteTemplateMatcher = new TemplateMatcher(TemplateParser.Parse(RoutePrefix + "/"+bd.Template), new RouteValueDictionary())
                    });
                }
            }
        }
        List<gTimedTaskTemplateMatcher> _templates = new List<gTimedTaskTemplateMatcher>();
        public async Task<bool> Dispatch(HttpContext context)
        {
            var httpRequest = context.Request;
            httpRequest.EnableBuffering();
            bool result = false;
            foreach (var x in _templates)
            {
                if (x.RouteTemplateMatcher.TryMatch(httpRequest.Path, httpRequest.RouteValues) && new HttpMethod(httpRequest.Method) == x.ApiHttpMethod)
                {
                    var apiTargetType = x.ApiTargetType;
                    var me = apiTargetType.GetMethod(x.Name);
                    var par = await GetParam(httpRequest, me);
                    var targetInstance = context.RequestServices.GetService(apiTargetType);
                    me.Invoke(targetInstance, par);
                    result = true;
                    break;
                }
            };
            return result;
        }

        public async Task<object[]> GetParam(HttpRequest request, MethodInfo me)
        {
            List<object> obj = new List<object>();
            ParameterInfo[] p = me.GetParameters();
            if (request.ContentLength == 0 || p == null || p.Length == 0)
            {
                return null;
            }
            if (request.Method.ToUpper() == "POST")
            {
                using (var reader = new StreamReader(request.Body, Encoding.UTF8))
                {
                    var a = await reader.ReadToEndAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    var job = System.Text.Json.JsonSerializer.Deserialize(a, p[0].ParameterType, options);
                    obj.Add(job);
                  //  request.Body.Position = 0;
                }
            }
            else if (request.Method.ToUpper() == "GET")
            {
                foreach (var item in p)
                {
                    obj.Add(request.Query.ContainsKey(item.Name) ? request.Query[item.Name] : item.DefaultValue);
                }
            }
            if (obj == null || obj.Count == 0)
            {
                return null;
            }
            return obj.ToArray();
        }
    }

    public class gTimedTaskTemplateMatcher
    {
        public HttpMethod ApiHttpMethod { get; set; }
        public TemplateMatcher RouteTemplateMatcher { get; set; }
        public string Name { get; set; }
        public Type ApiTargetType { get; set; }
    }
}
