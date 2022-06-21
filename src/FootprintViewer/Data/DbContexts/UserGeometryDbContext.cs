using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
#nullable disable

namespace FootprintViewer.Data
{
    public class UserGeometryDbContext : DbCustomContext
    {
        public DbSet<UserGeometry> UserGeometries { get; set; }

        public UserGeometryDbContext(string tableName, DbContextOptions<UserGeometryDbContext> options) : base(tableName, options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<UserGeometry>().ToTable(TableName);

            // UserGeometries
            modelBuilder.Entity<UserGeometry>(UserGeometriesConfigure);
        }

        protected static void UserGeometriesConfigure(EntityTypeBuilder<UserGeometry> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
            builder.Property(e => e.Type).HasConversion(
                v => v.ToString(),
                v => (UserGeometryType)Enum.Parse(typeof(UserGeometryType), v));
        }
    }
}
