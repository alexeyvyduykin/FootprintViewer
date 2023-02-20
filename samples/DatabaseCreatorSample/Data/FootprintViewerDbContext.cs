using FootprintViewer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DatabaseCreatorSample.Data;

public class FootprintViewerDbContext : DbContext
{
    public DbSet<Satellite> Satellites { get; set; }
    public DbSet<GroundTarget> GroundTargets { get; set; }
    public DbSet<Footprint> Footprints { get; set; }
    public DbSet<GroundStation> GroundStations { get; set; }
    public DbSet<UserGeometry> UserGeometries { get; set; }

    public FootprintViewerDbContext(DbContextOptions<FootprintViewerDbContext> options) : base(options)
    {
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    var options = optionsBuilder.Options;

    //    optionsBuilder.UseNpgsql("Host=localhost;Database=my_db2;Username=postgres;Password=user",
    //        options => options.SetPostgresVersion(new Version(9, 6)));
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        // Satellites
        modelBuilder.Entity<Satellite>(SatelliteConfigure);

        // GroudnTargets
        modelBuilder.Entity<GroundTarget>(GroundTargetConfigure);

        // Footprints
        modelBuilder.Entity<Footprint>(FootprintConfigure);

        // GroundStations
        modelBuilder.Entity<GroundStation>(GroundStationConfigure);

        // UserGeometries
        modelBuilder.Entity<UserGeometry>(UserGeometryConfigure);
    }

    protected void SatelliteConfigure(EntityTypeBuilder<Satellite> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
        builder.Property(e => e.Epoch).HasColumnType("timestamp without time zone");
    }

    protected void GroundTargetConfigure(EntityTypeBuilder<GroundTarget> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
        builder.Property(e => e.Type).HasConversion(
            v => v.ToString(),
            v => (GroundTargetType)Enum.Parse(typeof(GroundTargetType), v));
    }

    protected void FootprintConfigure(EntityTypeBuilder<Footprint> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
        builder.Property(e => e.Begin).HasColumnType("timestamp without time zone");
        builder.Property(e => e.Direction).HasConversion(
            v => v.ToString(),
            v => (SwathDirection)Enum.Parse(typeof(SwathDirection), v));
    }

    protected void GroundStationConfigure(EntityTypeBuilder<GroundStation> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
    }

    protected void UserGeometryConfigure(EntityTypeBuilder<UserGeometry> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
        builder.Property(e => e.Type).HasConversion(
            v => v.ToString(),
            v => (UserGeometryType)Enum.Parse(typeof(UserGeometryType), v));
    }

    public void AddModel(SceneModel model)
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();

        Satellites.AddRange(model.Satellites);
        GroundTargets.AddRange(model.GroundTargets);
        Footprints.AddRange(model.Footprints);
        GroundStations.AddRange(model.GroundStations);

        SaveChanges();
    }
}
