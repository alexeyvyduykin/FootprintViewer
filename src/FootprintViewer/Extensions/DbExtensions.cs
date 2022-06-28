using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace FootprintViewer
{
    public static class DbExtensions
    {
        public static string ToConnectionString(this IDatabaseSourceInfo info)
        {
            return $"Host={info.Host};Port={info.Port};Database={info.Database};Username={info.Username};Password={info.Password}";
        }

        public static DbContextOptions<T> BuildDbContextOptions<T>(this IDatabaseSourceInfo info) where T : DbCustomContext
        {
            var connectionString = info.ToConnectionString();
            var res = info.Version!.Split(new[] { '.' });
            var major = int.Parse(res[0]);
            var minor = int.Parse(res[1]);

            var optionsBuilder = new DbContextOptionsBuilder<T>();

            var options = optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.SetPostgresVersion(new Version(major, minor));
                options.UseNetTopologySuite();
            }).Options;

            return options;
        }
    }
}
