using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
#nullable disable

namespace FootprintViewer.Data
{
    public class GroundTargetDbContext : DbCustomContext
    {
        public DbSet<GroundTarget> GroundTargets { get; set; }

        public GroundTargetDbContext(string tableName, DbContextOptions<DbCustomContext> options) : base(tableName, options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<GroundTarget>().ToTable(TableName);

            // GroudnTargets
            modelBuilder.Entity<GroundTarget>(GroundTargetConfigure);
        }

        protected static void GroundTargetConfigure(EntityTypeBuilder<GroundTarget> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
            builder.Property(e => e.Type).HasConversion(
                v => v.ToString(),
                v => (GroundTargetType)Enum.Parse(typeof(GroundTargetType), v));
        }
    }
}
