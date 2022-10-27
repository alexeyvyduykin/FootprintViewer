using DataSettingsSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable disable

namespace DataSettingsSample.Data
{
    public class GroundStationDbContext : DbCustomContext
    {
        public DbSet<GroundStation> GroundStations { get; set; }

        public GroundStationDbContext(string connectionString, string tableName/*, DbContextOptions<GroundStationDbContext> options*/) : base(connectionString, tableName/*, options*/)
        {

        }
        public override IQueryable<object> GetTable() => GroundStations.Cast<object>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroundStation>().ToTable(TableName);

            // GroundStations
            modelBuilder.Entity<GroundStation>(GroundStationsConfigure);
        }

        protected static void GroundStationsConfigure(EntityTypeBuilder<GroundStation> builder)
        {
            builder.Property(b => b.Value).IsRequired();
            builder.HasKey(b => b.Value);
        }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //        => optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=user;Database=DataSettingsSampleDatabase2");

        public override async Task<IList<object>> ToListAsync() => await GroundStations.Cast<object>().ToListAsync().ConfigureAwait(false);
    }
}
