using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FootprintViewer.Data.Databases;

public class FootprintViewerDatabase : DbContext
{
    public DbSet<UserGeometry> UserGeometries => Set<UserGeometry>();

    public DbSet<PlannedScheduleResult> PlannedSchedules => Set<PlannedScheduleResult>();

    public FootprintViewerDatabase(DbContextOptions<FootprintViewerDatabase> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        // UserGeometries
        modelBuilder.Entity<UserGeometry>(UserGeometryDbContext.Configure);

        // PlannedSchedules
        modelBuilder.Entity<PlannedScheduleResult>(PlannedScheduleDbContext.Configure);
    }
}
