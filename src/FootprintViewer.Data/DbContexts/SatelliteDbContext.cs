using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootprintViewer.Data.DbContexts;

public class SatelliteDbContext : DbCustomContext
{
    public DbSet<Satellite> Satellites => Set<Satellite>();

    //public SatelliteDbContext(string tableName, DbContextOptions<SatelliteDbContext> options) : base(tableName, options)
    //{

    //}

    public SatelliteDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    public static Action<EntityTypeBuilder<Satellite>> Configure => s => SatelliteConfigureImpl(s);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<Satellite>().ToTable(TableName);

        // Satellites
        modelBuilder.Entity<Satellite>(Configure);
    }

    protected static void SatelliteConfigureImpl(EntityTypeBuilder<Satellite> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);

        builder.Property(e => e.Epoch).HasColumnType("timestamp without time zone");
    }

    public override IQueryable<object> GetTable() => Satellites.Cast<object>();
}
