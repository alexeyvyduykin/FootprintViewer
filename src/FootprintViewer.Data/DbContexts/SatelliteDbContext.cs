﻿using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootprintViewer.Data.DbContexts;

public class SatelliteDbContext : DbCustomContext
{
    public DbSet<Satellite> Satellites => Set<Satellite>();

    public SatelliteDbContext(string tableName, DbContextOptions<SatelliteDbContext> options) : base(tableName, options)
    {

    }

    public SatelliteDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<Satellite>().ToTable(TableName);

        // Satellites
        modelBuilder.Entity<Satellite>(SatelliteConfigure);
    }

    protected static void SatelliteConfigure(EntityTypeBuilder<Satellite> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
    }

    public override IQueryable<object> GetTable() => Satellites.Cast<object>();
}
