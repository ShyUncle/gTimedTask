using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace gTimedTask.Core.DomainModel
{
    [Table("GTIMEDTASK_JOB")]
    public class JobEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Cron { get; set; }
        public int State { get; set; }
        public string ExecutorGroup { get; set; }
        public string ExecutorHandler { get; set; }
        public int ExecutorType { get; set; }
        public string ExecutorParam { get; set; }
        public long CreateTime { get; set; }
    }
}
