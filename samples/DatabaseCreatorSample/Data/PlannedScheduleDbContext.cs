using DatabaseCreatorSample.Data.Models;
using FootprintViewer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DatabaseCreatorSample.Data;

internal class PlannedScheduleDbContext : DbContext
{
    public DbSet<Satellite> Satellites { get; set; }

    public DbSet<GroundTarget> GroundTargets { get; set; }

    public DbSet<GroundStation> GroundStations { get; set; }

    public DbSet<UserGeometry> UserGeometries { get; set; }

    public DbSet<ObservationTask> ObservationTasks { get; set; }

    public DbSet<CommunicationTask> CommunicationTasks { get; set; }

    public DbSet<TransitionTask> TransitionTasks { get; set; }

    public PlannedScheduleDbContext(DbContextOptions<PlannedScheduleDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        // Satellites
        modelBuilder.Entity<Satellite>(SatelliteConfigure);

        // GroudnTargets
        modelBuilder.Entity<GroundTarget>(GroundTargetConfigure);

        // ObservationTasks
        modelBuilder.Entity<ObservationTask>(ObservationTaskConfigure);

        // CommunicationTasks
        modelBuilder.Entity<CommunicationTask>(CommunicationTaskConfigure);

        // TransitionTasks
        modelBuilder.Entity<TransitionTask>(TransitionTaskConfigure);

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

    protected void TaskConfigure<T>(EntityTypeBuilder<T> builder) where T : BaseTask
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);

        builder.Property(e => e.ActiveBeginTime).HasColumnType("timestamp without time zone");
        builder.Property(e => e.AvailableBeginTime).HasColumnType("timestamp without time zone");
    }

    protected void ObservationTaskConfigure(EntityTypeBuilder<ObservationTask> builder)
    {
        TaskConfigure(builder);

        builder.Property(e => e.Direction).HasConversion(
            v => v.ToString(),
            v => (SwathDirection)Enum.Parse(typeof(SwathDirection), v));
    }

    protected void CommunicationTaskConfigure(EntityTypeBuilder<CommunicationTask> builder)
    {
        TaskConfigure(builder);
    }

    protected void TransitionTaskConfigure(EntityTypeBuilder<TransitionTask> builder)
    {
        TaskConfigure(builder);
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
        GroundStations.AddRange(model.GroundStations);

        ObservationTasks.AddRange(model.ObservationTasks);
        CommunicationTasks.AddRange(model.CommunicationTasks);
        TransitionTasks.AddRange(model.TransitionTasks);

        SaveChanges();
    }
}
