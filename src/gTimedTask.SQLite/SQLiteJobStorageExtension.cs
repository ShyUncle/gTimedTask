using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using gTimedTask.Core.Storage;
namespace gTimedTask.SQLite
{
    public static class SQLiteJobStorageExtension
    {
        public static IServiceCollection UseSQLiteJobStorage(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IJobStorageConnection, SQLiteJobStorage>((provider) => new SQLiteJobStorage(connectionString));
            return services;
        }
    }
}
