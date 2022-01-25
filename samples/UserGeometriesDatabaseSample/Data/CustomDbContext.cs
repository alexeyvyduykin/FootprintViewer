using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace UserGeometriesDatabaseSample.Data
{
    public class CustomDbContext : DbContext
    {
        public DbSet<UserGeometry> UserGeometries { get; set; }

        public CustomDbContext(DbContextOptions<CustomDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var options = optionsBuilder.Options;

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=FootprintViewerDatabase;Username=postgres;Password=user",
                options => options.SetPostgresVersion(new Version(14, 1)));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            // UserGeometries
            modelBuilder.Entity<UserGeometry>(UserGeometriesConfigure);
        }

        protected void UserGeometriesConfigure(EntityTypeBuilder<UserGeometry> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
            builder.Property(e => e.Type).HasConversion(
                v => v.ToString(),
                v => (UserGeometryType)Enum.Parse(typeof(UserGeometryType), v));
        }
    }
}
