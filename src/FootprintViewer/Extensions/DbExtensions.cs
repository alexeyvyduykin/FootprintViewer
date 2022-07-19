using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace FootprintViewer
{
    public static class DbExtensions
    {
        public static string ToConnectionString(this IDatabaseSourceViewModel vm)
        {
            return $"Host={vm.Host};Port={vm.Port};Database={vm.Database};Username={vm.Username};Password={vm.Password}";
        }

        public static DbContextOptions<T> BuildDbContextOptions<T>(this IDatabaseSourceViewModel vm) where T : DbCustomContext
        {
            var connectionString = vm.ToConnectionString();
            var res = vm.Version!.Split(new[] { '.' });
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
