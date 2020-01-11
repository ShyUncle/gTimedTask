using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using gTimedTask.Core.DomainModel;
using System.Collections.Generic;

namespace gTimedTask.Core.Api
{
    public interface IUserService
    {
        [gTimedTaskRoute("user/login", "Post")]
        Task<object> Login(user userName);

        [gTimedTaskRoute("user/info")]
        Task<object> Info(string token);
    }

    public class user
    {
        public string UserName { get; set; }
    }
}
