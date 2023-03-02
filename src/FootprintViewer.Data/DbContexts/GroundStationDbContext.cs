using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootprintViewer.Data.DbContexts;

public class GroundStationDbContext : DbCustomContext
{
    public DbSet<GroundStation> GroundStations => Set<GroundStation>();

    //public GroundStationDbContext(string tableName, DbContextOptions<GroundStationDbContext> options) : base(tableName, options)
    //{

    //}

    public GroundStationDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    public static Action<EntityTypeBuilder<GroundStation>> Configure => s => GroundStationsConfigureImpl(s);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<GroundStation>().ToTable(TableName);

        // GroundStations
        modelBuilder.Entity<GroundStation>(Configure);
    }

    protected static void GroundStationsConfigureImpl(EntityTypeBuilder<GroundStation> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
    }

    public override IQueryable<object> GetTable() => GroundStations.Cast<object>();
}
