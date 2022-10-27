using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public class DatabaseSource : BaseSource
    {
        private readonly DbKeys _key;
        private readonly string _connectionString;
        private readonly string _tableName;

        public DatabaseSource(DbKeys key, string connectionString, string tableName)
        {
            _key = key;
            _connectionString = connectionString;
            _tableName = tableName;
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
            await using var context = DbHelper.CreateDatabaseSource(_key, _tableName).Invoke(_connectionString);

            return await context.GetTable().ToListAsync();
        }
    }

    public static class extns2
    {
        public static string ToConnectionString(string host, int port, string database, string username, string password)
        {
            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        public static DbContextOptions<T> BuildDbContextOptions<T>(string connectionString) where T : DbCustomContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();

            var options = optionsBuilder.UseNpgsql(connectionString).Options;

            return options;
        }
    }
}
