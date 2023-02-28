using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FootprintViewer.Data;

public class PlannedScheduleDbContext : DbCustomContext
{
    public DbSet<PlannedScheduleResult> PlannedSchedules => Set<PlannedScheduleResult>();

    public PlannedScheduleDbContext(string tableName, DbContextOptions<PlannedScheduleDbContext> options) : base(tableName, options)
    {

    }

    public PlannedScheduleDbContext(string tableName, string connectionString) : base(tableName, connectionString)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<PlannedScheduleResult>().ToTable(TableName);

        // PlannedSchedules
        modelBuilder.Entity<PlannedScheduleResult>(PlannedScheduleConfigure);
    }

    protected static void PlannedScheduleConfigure(EntityTypeBuilder<PlannedScheduleResult> builder)
    {
        builder.Property(b => b.Name).IsRequired();
        builder.HasKey(b => b.Name);

        builder.Property(e => e.DateTime).HasColumnType("timestamp without time zone");

        builder.Property(e => e.Tasks).HasConversion(
            v => SerializeObject(v),
            v => DeserializeObject<List<ITask>>(v) ?? new());

        builder.Property(e => e.PlannedSchedules).HasConversion(
            v => SerializeObject(v),
            v => DeserializeObject<List<ITaskResult>>(v) ?? new());
    }

    public override IQueryable<object> GetTable() => PlannedSchedules.Cast<object>();

    private static string SerializeObject(object? value)
    {
        var serializer = GeoJsonSerializer.Create();
        using var stringWriter = new StringWriter();
        using var jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Serialize(jsonWriter, value);
        return stringWriter.ToString();

        //return JsonConvert.SerializeObject(value,
        //    new JsonSerializerSettings
        //    {
        //        NullValueHandling = NullValueHandling.Ignore
        //    });
    }

    private static T? DeserializeObject<T>(string value)
    {
        var serializer = GeoJsonSerializer.Create();

        using var stringReader = new StringReader(value);
        using var jsonReader = new JsonTextReader(stringReader);

        return serializer.Deserialize<T>(jsonReader);

        //return JsonConvert.DeserializeObject<T>(value,
        //    new JsonSerializerSettings
        //    {
        //        NullValueHandling = NullValueHandling.Ignore
        //    });
    }
}
