using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootprintViewer.Data.DbContexts;

public class GroundTargetDbContext : DbCustomContext
{
    public DbSet<GroundTarget> GroundTargets => Set<GroundTarget>();

    //public GroundTargetDbContext(string tableName, DbContextOptions<GroundTargetDbContext> options) : base(tableName, options)
    //{

    //}

    public GroundTargetDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    public static Action<EntityTypeBuilder<GroundTarget>> Configure => s => GroundTargetConfigureImpl(s);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<GroundTarget>().ToTable(TableName);

        // GroudnTargets
        modelBuilder.Entity<GroundTarget>(Configure);
    }

    protected static void GroundTargetConfigureImpl(EntityTypeBuilder<GroundTarget> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
        builder.Property(e => e.Type).HasConversion(
            v => v.ToString(),
            v => (GroundTargetType)Enum.Parse(typeof(GroundTargetType), v));
    }

    public override IQueryable<object> GetTable() => GroundTargets.Cast<object>();
}
