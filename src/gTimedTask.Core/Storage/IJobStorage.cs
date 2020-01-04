using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace gTimedTask.Core.Storage
{
    public interface IJobStorageConnection
    {
        public IDbConnection GetDbConnection();
    }
}
