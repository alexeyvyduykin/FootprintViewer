﻿using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FootprintViewer.Data.Databases;

public class PlannedScheduleDatabase : DbContext
{
    public DbSet<Satellite> Satellites => Set<Satellite>();

    public DbSet<GroundTarget> GroundTargets => Set<GroundTarget>();

    public DbSet<GroundStation> GroundStations => Set<GroundStation>();

    public DbSet<UserGeometry> UserGeometries => Set<UserGeometry>();

    public DbSet<PlannedScheduleResult> PlannedSchedules => Set<PlannedScheduleResult>();

    public PlannedScheduleDatabase(DbContextOptions<PlannedScheduleDatabase> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        // Satellites
        modelBuilder.Entity<Satellite>(SatelliteDbContext.Configure);

        // GroudnTargets
        modelBuilder.Entity<GroundTarget>(GroundTargetDbContext.Configure);

        // GroundStations
        modelBuilder.Entity<GroundStation>(GroundStationDbContext.Configure);

        // UserGeometries
        modelBuilder.Entity<UserGeometry>(UserGeometryDbContext.Configure);

        // PlannedSchedules
        modelBuilder.Entity<PlannedScheduleResult>(PlannedScheduleDbContext.Configure);
    }
}