using gTimedTask.Core.DomainModel;
using gTimedTask.Core.Storage;
using System;
using System.Linq;
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
            var result = await DynamicJobScheduler.AddJob(jobEntity.ExecutorHandler, jobEntity.ExecutorGroup, jobEntity.Cron);

            if (result)
            {
                result = await _dbRepository.InsertAsync(jobEntity) > 0;
            }

            return result;
        }

        public async Task<bool> Delete(long id)
        {
            var jobEntity = await Get(id);
            var result = await DynamicJobScheduler.RemoveJob(jobEntity.ExecutorHandler, jobEntity.ExecutorGroup);

            if (result)
            {
                result = await _dbRepository.DeleteAsync(jobEntity);
            }

            return result;
        }

        public async Task<JobEntity> Get(long id)
        {
            return await _dbRepository.GetAsync<JobEntity>(id);
        }

        public async Task<List<JobEntity>> Query(int pageIndex, int pageSize)
        {
            var list = await _dbRepository.GetAllAsync<JobEntity>();
            return list.ToList();
        }

        public async Task<bool> Trigger(long id)
        {
            var jobEntity = await Get(id);
            return await DynamicJobScheduler.TriggerJob(jobEntity.ExecutorHandler, jobEntity.ExecutorGroup);
        }

        public async Task<JobEntity> Update(JobEntity jobEntity)
        {
            var result = await DynamicJobScheduler.RescheduleJob(jobEntity.ExecutorHandler, jobEntity.ExecutorGroup, jobEntity.Cron);
            if (result)
            {
                await _dbRepository.UpdateAsync(jobEntity);
                return jobEntity;
            }
            return null;
        }

    }
}
