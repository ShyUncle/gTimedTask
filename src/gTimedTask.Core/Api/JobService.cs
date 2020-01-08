using gTimedTask.Core.DomainModel;
using gTimedTask.Core.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace gTimedTask.Core.Api
{
    public class JobService : IJobService
    {
        private DbRepository _dbRepository;

        public JobService(DbRepository dbRepository)
        {
            this._dbRepository = dbRepository;
        }
        public async Task<bool> Add(DomainModel.JobEntity jobEntity)
        {
            var result =await DynamicJobScheduler.AddJob(jobEntity.ExecutorHandler, jobEntity.ExecutorGroup, jobEntity.Cron);

            if (result)
            {
                result = await _dbRepository.InsertAsync(jobEntity) > 0;
            }
            return result;
        }

        public Task<bool> Update(JobEntity jobEntity)
        {
            throw new NotImplementedException();
        } 
    }
}
