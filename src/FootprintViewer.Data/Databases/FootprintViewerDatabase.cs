using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FootprintViewer.Data.Databases;

public class FootprintViewerDatabase : DbContext
{
    public DbSet<Footprint> Footprints => Set<Footprint>();

    public DbSet<GroundStation> GroundStations => Set<GroundStation>();

    public DbSet<UserGeometry> UserGeometries => Set<UserGeometry>();

    public FootprintViewerDatabase(DbContextOptions<FootprintViewerDatabase> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        // Footprints
        modelBuilder.Entity<Footprint>(FootprintDbContext.Configure);

        // GroundStations
        modelBuilder.Entity<GroundStation>(GroundStationDbContext.Configure);

        // UserGeometries
        modelBuilder.Entity<UserGeometry>(UserGeometryDbContext.Configure);
    }
}
