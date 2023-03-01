using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
    private readonly string? _connectionString;

    public DbCustomContext(string tableName, DbContextOptions options) : base(options)
    {
        _tableName = tableName;
    }

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

    public string TableName => _tableName;
}
