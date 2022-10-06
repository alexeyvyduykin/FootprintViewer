using DataSettingsSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#nullable disable

namespace DataSettingsSample.Data
{
    public class SatelliteDbContext : DbCustomContext
    {
        public DbSet<Satellite> Satellites { get; set; }

        public SatelliteDbContext(string tableName, DbContextOptions<SatelliteDbContext> options) : base(tableName, options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Satellite>().ToTable(TableName);

            // Satellites
            modelBuilder.Entity<Satellite>(SatelliteConfigure);
        }

        protected static void SatelliteConfigure(EntityTypeBuilder<Satellite> builder)
        {
            builder.Property(b => b.Value).IsRequired();
            builder.HasKey(b => b.Value);
        }
    }
}
