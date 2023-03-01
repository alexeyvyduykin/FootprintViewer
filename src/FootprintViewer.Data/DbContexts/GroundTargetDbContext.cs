﻿using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootprintViewer.Data.DbContexts;

public class GroundTargetDbContext : DbCustomContext
{
    public DbSet<GroundTarget> GroundTargets => Set<GroundTarget>();

    public GroundTargetDbContext(string tableName, DbContextOptions<DbCustomContext> options) : base(tableName, options)
    {

    }

    public GroundTargetDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
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

    public override IQueryable<object> GetTable() => GroundTargets.Cast<object>();
}
