using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace FootprintViewer.Data.DbContexts;

// TODO: https://stackoverflow.com/questions/51864015/entity-framework-map-model-class-to-table-at-run-time
class CustomModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context) => new CustomModelCacheKey(context);
}

class CustomModelCacheKey
{
    (Type ContextType, string CustomTableName) key;

    public CustomModelCacheKey(DbContext context)
    {
        key.ContextType = context.GetType();
        key.CustomTableName = ((DbCustomContext)context).TableName;
    }

    public override int GetHashCode() => key.GetHashCode();

    public override bool Equals(object? obj) => obj is CustomModelCacheKey other && key.Equals(other.key);
}

public abstract class DbCustomContext : DbContext
{
    private readonly string _tableName;
    private readonly string _connectionString;

    public DbCustomContext(string tableName, string connectionString) : base()
    {
        _tableName = tableName;
        _connectionString = connectionString;
    }

    public bool Valid(Type entityType)
    {
        try
        {
            var obj = GetTable().FirstOrDefault();

            return Equals(entityType.Name, obj?.GetType().Name);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Valid<TEntity>() where TEntity : class
    {
        try
        {
            var obj = GetTable().FirstOrDefault();

            return Equals(typeof(TEntity).Name, obj?.GetType().Name);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public abstract IQueryable<object> GetTable();

    public virtual async Task<IList<object>> GetValuesAsync()
    {
        return await GetTable().ToListAsync();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.ReplaceService<IModelCacheKeyFactory, CustomModelCacheKeyFactory>();

        if (string.IsNullOrEmpty(_connectionString) == false)
        {
            optionsBuilder.UseNpgsql(_connectionString, options =>
            {
                //options.SetPostgresVersion(new Version(major, minor));
                options.UseNetTopologySuite();
            });
        }
    }

    protected static string SerializeObject(object? value)
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

    protected static T? DeserializeObject<T>(string value)
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

    public string TableName => _tableName;

    public string ConnectionString => _connectionString;
}
