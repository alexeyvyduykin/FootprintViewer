using DataSettingsSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#nullable disable

namespace DataSettingsSample.Data
{
    public class GroundStationDbContext : DbCustomContext
    {
        public DbSet<GroundStation> GroundStations { get; set; }

        public GroundStationDbContext(string tableName, DbContextOptions<DbCustomContext> options) : base(tableName, options)
        {

        }

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
    }
}
