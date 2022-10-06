using DataSettingsSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#nullable disable

namespace DataSettingsSample.Data
{
    public class FootprintDbContext : DbCustomContext
    {
        public DbSet<Footprint> Footprints { get; set; }

        public FootprintDbContext(string tableName, DbContextOptions<FootprintDbContext> options) : base(tableName, options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Footprint>().ToTable(TableName);

            // Footprints
            modelBuilder.Entity<Footprint>(FootprintConfigure);
        }

        protected static void FootprintConfigure(EntityTypeBuilder<Footprint> builder)
        {
            builder.Property(b => b.Value).IsRequired();
            builder.HasKey(b => b.Value);
        }
    }
}
