using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootprintViewer.Data.DbContexts;

public class UserGeometryDbContext : DbCustomContext
{
    public DbSet<UserGeometry> UserGeometries => Set<UserGeometry>();

    //public UserGeometryDbContext(string tableName, DbContextOptions<UserGeometryDbContext> options) : base(tableName, options)
    //{

    //}

    public UserGeometryDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    public static Action<EntityTypeBuilder<UserGeometry>> Configure => s => UserGeometriesConfigureImpl(s);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<UserGeometry>().ToTable(TableName);

        // UserGeometries
        modelBuilder.Entity<UserGeometry>(Configure);
    }

    protected static void UserGeometriesConfigureImpl(EntityTypeBuilder<UserGeometry> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);
        builder.Property(e => e.Type).HasConversion(
            v => v.ToString(),
            v => (UserGeometryType)Enum.Parse(typeof(UserGeometryType), v));
    }

    public override IQueryable<object> GetTable() => UserGeometries.Cast<object>();
}
