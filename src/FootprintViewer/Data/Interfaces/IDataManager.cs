using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IDataManager<TNative>
    {
        Task<List<TNative>> GetNativeValuesAsync(IDataSource dataSource, IFilter<TNative>? filter);

        Task<List<T>> GetValuesAsync<T>(IDataSource dataSource, IFilter<T>? filter, Func<TNative, T> converter);
    }

    public static class extns2
    {
        public static string ToConnectionString(string host, int port, string database, string username, string password)
        {
            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        public static DbContextOptions<T2> BuildDbContextOptions<T2>(IDatabaseSource ds) where T2 : DbCustomContext
        {
            var connectionString = ToConnectionString(ds.Host, ds.Port, ds.Database, ds.Username, ds.Password);
            var res = ds.Version!.Split(new[] { '.' });
            var major = int.Parse(res[0]);
            var minor = int.Parse(res[1]);

            var optionsBuilder = new DbContextOptionsBuilder<T2>();

            var options = optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.SetPostgresVersion(new Version(major, minor));
                options.UseNetTopologySuite();
            }).Options;

            return options;
        }

    }
}
