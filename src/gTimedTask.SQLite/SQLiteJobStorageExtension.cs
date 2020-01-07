using gTimedTask.Core.Storage;
using Microsoft.Extensions.DependencyInjection;
namespace gTimedTask.SQLite
{
    public static class SQLiteJobStorageExtension
    {
        public static IServiceCollection AddSQLiteJobStorage(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IJobStorageConnection, SQLiteJobStorage>((provider) => new SQLiteJobStorage(connectionString));
            return services;
        }
    }
}
