using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

    public static class SatelliteMap
    {
        public static ModelBuilder MapSatellite(this ModelBuilder modelBuilder, string tableName)
        {
            var entity = modelBuilder.Entity<Satellite>();

            entity.ToTable(tableName);

            entity.Property(b => b.Name).IsRequired();
            entity.HasKey(b => b.Name);

            return modelBuilder;
        }
    }


    // TODO: https://stackoverflow.com/questions/51864015/entity-framework-map-model-class-to-table-at-run-time
    class CustomModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context) => new CustomModelCacheKey(context);
    }

    class CustomModelCacheKey
    {
        (Type ContextType, string CustomTableName) key;

        public CustomModelCacheKey(DbContext context)
        {
            key.ContextType = context.GetType();
            key.CustomTableName = (context as SatelliteDbContext)?.TableName;
        }

        public override int GetHashCode() => key.GetHashCode();

        public override bool Equals(object obj) => obj is CustomModelCacheKey other && key.Equals(other.key);
    }

    public class SatelliteDbContext : DbContext
    {
        private readonly string _tableName;

        public DbSet<Satellite> Satellites { get; set; }

        public SatelliteDbContext(string tableName, DbContextOptions<SatelliteDbContext> options) : base(options)
        {
            _tableName = tableName;
        }

        public string TableName => _tableName;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ReplaceService<IModelCacheKeyFactory, CustomModelCacheKeyFactory>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.CacheForContextType = false;

            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<Satellite>().ToTable(_tableName);

            // Satellites
            modelBuilder.Entity<Satellite>(SatelliteConfigure);
        }

        protected static void SatelliteConfigure(EntityTypeBuilder<Satellite> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
        }
    }

    public class UserGeometryDbContext : DbContext
    {
        public DbSet<UserGeometry> UserGeometries { get; set; }

        public UserGeometryDbContext(DbContextOptions<UserGeometryDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

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
