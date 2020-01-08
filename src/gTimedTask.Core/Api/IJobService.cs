using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace gTimedTask.Core.Api
{
    public interface IJobService
    {
        [gTimedTaskRoute("/job/add", "post")]
        Task<bool> Add(DomainModel.JobEntity jobEntity);

        [gTimedTaskRoute("/job/update")]
        Task<bool> Update(DomainModel.JobEntity jobEntity);
    }
}
