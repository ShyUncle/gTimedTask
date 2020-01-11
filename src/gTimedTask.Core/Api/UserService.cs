using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace gTimedTask.Core.Api
{
    public class UserService : IUserService
    {

        public async Task<object> Login(user userName)
        {
            return await Task.FromResult(new { token = "admin-token" });
        }

        public async Task<object> Info(string userName)
        {
            return await Task.FromResult(new
            {
                roles = new List<string>() { "admin" },
                introduction = "I am a super administra",
                name = "Super Admin",
                avatar = "https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif"

            });
        }
    }
}
