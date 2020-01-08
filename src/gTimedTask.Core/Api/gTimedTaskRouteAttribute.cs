using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace gTimedTask.Core.Api
{
    [AttributeUsage(AttributeTargets.Method)]
    public class gTimedTaskRouteAttribute : Attribute
    {
        public string Template { get; private set; }
        public HttpMethod ApiHttpMethod { get; private set; }
        public gTimedTaskRouteAttribute(string template, string httpMethod = "GET")
        {
            Template = template;
            ApiHttpMethod = new HttpMethod(httpMethod);
        }
    }
}
