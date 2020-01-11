using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using gTimedTask.Core.DomainModel;
using System.Collections.Generic;

namespace gTimedTask.Core.Api
{
    public interface IJobService
    {
        [gTimedTaskRoute("job/query")]
        Task<List<JobEntity>> Query(int pageIndex,int pageSize);

        [gTimedTaskRoute("job/add", "post")]
        Task<bool> Add(JobEntity jobEntity);

        [gTimedTaskRoute("job/update", "Post")]
        Task<JobEntity> Update(JobEntity jobEntity);


        [gTimedTaskRoute("job/get")]
        Task<JobEntity> Get(long id);

        [gTimedTaskRoute("job/delete")]
        Task<bool> Delete(long id);

        [gTimedTaskRoute("job/trigger")]
        Task<bool> Trigger(long id);
    }
}
