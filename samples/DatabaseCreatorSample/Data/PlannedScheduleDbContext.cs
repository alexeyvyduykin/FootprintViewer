using FootprintViewer.Data;
using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DatabaseCreatorSample.Data;

internal class PlannedScheduleDbContext : DbContext
{
    public DbSet<Satellite> Satellites => Set<Satellite>();

    public DbSet<GroundTarget> GroundTargets => Set<GroundTarget>();

    public DbSet<GroundStation> GroundStations => Set<GroundStation>();

    public DbSet<UserGeometry> UserGeometries => Set<UserGeometry>();

    public DbSet<PlannedScheduleResult> PlannedSchedules => Set<PlannedScheduleResult>();

    public PlannedScheduleDbContext(DbContextOptions<PlannedScheduleDbContext> options) : base(options) { }

    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        // Satellites
        modelBuilder.Entity<Satellite>(SatelliteConfigure);

        // GroudnTargets
        modelBuilder.Entity<GroundTarget>(GroundTargetConfigure);

        // GroundStations
        modelBuilder.Entity<GroundStation>(GroundStationConfigure);

        // UserGeometries
        modelBuilder.Entity<UserGeometry>(UserGeometryConfigure);

        // PlannedSchedules
        modelBuilder.Entity<PlannedScheduleResult>(PlannedScheduleRecordConfigure);
    }

    protected void PlannedScheduleRecordConfigure(EntityTypeBuilder<PlannedScheduleResult> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);

        builder.Property(e => e.DateTime).HasColumnType("timestamp without time zone");

        builder.Property(e => e.Tasks).HasConversion(
            v => SerializeObject(v),
            v => DeserializeObject<List<ITask>>(v) ?? new());

        builder.Property(e => e.TaskAvailabilities).HasConversion(
            v => SerializeObject(v),
            v => DeserializeObject<List<TaskAvailability>>(v) ?? new());

        builder.Property(e => e.PlannedSchedules).HasConversion(
            v => SerializeObject(v),
            v => DeserializeObject<List<ITaskResult>>(v) ?? new());
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

        var observations = model.ObservationTasks;
        var satellites = model.Satellites;
        var tasks = model.Tasks;

        Satellites.AddRange(satellites);
        GroundTargets.AddRange(model.GroundTargets);
        GroundStations.AddRange(model.GroundStations);

        var res = new PlannedScheduleResult()
        {
            Name = "PlannedSchedule1",
            DateTime = DateTime.Now,
            Tasks = tasks,
            PlannedSchedules = observations
        };

        PlannedSchedules.Add(res);

        SaveChanges();
    }

    private static string SerializeObject(object? value)
    {
        var serializer = GeoJsonSerializer.Create();
        using var stringWriter = new StringWriter();
        using var jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Serialize(jsonWriter, value);
        return stringWriter.ToString();

        //return JsonConvert.SerializeObject(value,
        //    new JsonSerializerSettings
        //    {
        //        NullValueHandling = NullValueHandling.Ignore
        //    });
    }

    private static T? DeserializeObject<T>(string value)
    {
        var serializer = GeoJsonSerializer.Create();

        using var stringReader = new StringReader(value);
        using var jsonReader = new JsonTextReader(stringReader);

        return serializer.Deserialize<T>(jsonReader);

        //return JsonConvert.DeserializeObject<T>(value,
        //    new JsonSerializerSettings
        //    {
        //        NullValueHandling = NullValueHandling.Ignore
        //    });
    }
}
