using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
#nullable disable

namespace FootprintViewer.Data;

public class FootprintDbContext : DbCustomContext
{
    public DbSet<Footprint> Footprints { get; set; }

    public FootprintDbContext(string tableName, DbContextOptions<FootprintDbContext> options) : base(tableName, options)
    {

    }

    public FootprintDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<Footprint>().ToTable(TableName);

        // Footprints
        modelBuilder.Entity<Footprint>(FootprintConfigure);
    }

    protected static void FootprintConfigure(EntityTypeBuilder<Footprint> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
        builder.Property(e => e.Direction).HasConversion(
            v => v.ToString(),
            v => (SwathDirection)Enum.Parse(typeof(SwathDirection), v));
    }

    public override IQueryable<object> GetTable() => Footprints.Cast<object>();
}
