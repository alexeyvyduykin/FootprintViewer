using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
#nullable disable

namespace FootprintViewer.Data
{
    public class GroundStationDbContext : DbContext
    {
        public DbSet<GroundStation> GroundStations { get; set; }

        public GroundStationDbContext(DbContextOptions<GroundStationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            // GroundStations
            modelBuilder.Entity<GroundStation>(GroundStationsConfigure);
        }

        protected static void GroundStationsConfigure(EntityTypeBuilder<GroundStation> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
        }
    }

    public class GroundTargetDbContext : DbContext
    {
        public DbSet<GroundTarget> GroundTargets { get; set; }

        public GroundTargetDbContext(DbContextOptions<GroundTargetDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

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

    public class FootprintDbContext : DbContext
    {
        public DbSet<Footprint> Footprints { get; set; }

        public FootprintDbContext(DbContextOptions<FootprintDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            // Footprints
            modelBuilder.Entity<Footprint>(FootprintConfigure);
        }

        protected static void FootprintConfigure(EntityTypeBuilder<Footprint> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
            builder.Property(e => e.Direction).HasConversion(
                v => v.ToString(),
                v => (SatelliteStripDirection)Enum.Parse(typeof(SatelliteStripDirection), v));
        }
    }

    public class FootprintViewerDbContext : DbContext
    {
        public DbSet<Satellite> Satellites { get; set; }
        public DbSet<UserGeometry> UserGeometries { get; set; }

        public FootprintViewerDbContext(DbContextOptions<FootprintViewerDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            // Satellites
            modelBuilder.Entity<Satellite>(SatelliteConfigure);

            // UserGeometries
            modelBuilder.Entity<UserGeometry>(UserGeometriesConfigure);
        }

        protected static void SatelliteConfigure(EntityTypeBuilder<Satellite> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
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
