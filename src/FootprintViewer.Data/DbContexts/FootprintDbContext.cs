using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootprintViewer.Data.DbContexts;

public class FootprintDbContext : DbCustomContext
{
    public DbSet<Footprint> Footprints => Set<Footprint>();

    //public FootprintDbContext(string tableName, DbContextOptions<FootprintDbContext> options) : base(tableName, options)
    //{

    //}

    public FootprintDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    public static Action<EntityTypeBuilder<Footprint>> Configure => s => FootprintConfigureImpl(s);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<Footprint>().ToTable(TableName);

        // Footprints
        modelBuilder.Entity<Footprint>(Configure);
    }

    protected static void FootprintConfigureImpl(EntityTypeBuilder<Footprint> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
        builder.Property(e => e.Begin).HasColumnType("timestamp without time zone");
        builder.Property(e => e.Direction).HasConversion(
            v => v.ToString(),
            v => (SwathDirection)Enum.Parse(typeof(SwathDirection), v));
    }

    public override IQueryable<object> GetTable() => Footprints.Cast<object>();
}
