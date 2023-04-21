using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootprintViewer.Data.DbContexts;

public class PlannedScheduleDbContext : DbCustomContext
{
    public DbSet<PlannedScheduleResult> PlannedSchedules => Set<PlannedScheduleResult>();

    public PlannedScheduleDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    public static Action<EntityTypeBuilder<PlannedScheduleResult>> Configure => s => PlannedScheduleConfigureImpl(s);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<PlannedScheduleResult>().ToTable(TableName);

        // PlannedSchedules
        modelBuilder.Entity<PlannedScheduleResult>(Configure);
    }

    protected static void PlannedScheduleConfigureImpl(EntityTypeBuilder<PlannedScheduleResult> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);

        builder.Property(e => e.DateTime).HasColumnType("timestamp without time zone");

        builder.Property(e => e.Satellites).HasConversion(
            v => SerializeObject(v),
            v => DeserializeObject<List<Satellite>>(v) ?? new());

        builder.Property(e => e.GroundTargets).HasConversion(
            v => SerializeObject(v),
            v => DeserializeObject<List<GroundTarget>>(v) ?? new());

        builder.Property(e => e.GroundStations).HasConversion(
            v => SerializeObject(v),
            v => DeserializeObject<List<GroundStation>>(v) ?? new());

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

    public override IQueryable<object> GetTable() => PlannedSchedules.Cast<object>();
}
