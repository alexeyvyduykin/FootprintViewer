using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
#nullable disable

namespace FootprintViewer.Data
{
    public class GroundStationDbContext : DbCustomContext
    {
        public DbSet<GroundStation> GroundStations { get; set; }

        public GroundStationDbContext(string tableName, DbContextOptions<DbCustomContext> options) : base(tableName, options)
        {

        }

        public GroundStationDbContext(string tableName, string connectionString) : base(tableName, connectionString)
        {

        }

        protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<GroundStation>().ToTable(TableName);

            // GroundStations
            modelBuilder.Entity<GroundStation>(GroundStationsConfigure);
        }

        protected static void GroundStationsConfigure(EntityTypeBuilder<GroundStation> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
        }

        public override IQueryable<object> GetTable() => GroundStations.Cast<object>();
    }
}
