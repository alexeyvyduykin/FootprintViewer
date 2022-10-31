using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable disable

namespace DataSettingsSample.Data
{
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
            key.CustomTableName = (context as DbCustomContext)?.TableName;
        }

        public override int GetHashCode() => key.GetHashCode();

        public override bool Equals(object obj) => obj is CustomModelCacheKey other && key.Equals(other.key);
    }

    public interface IDbCustomContext : IDisposable
    {
        Task<IList<object>> ToListAsync();

        IQueryable<object> GetTable();
    }

    public abstract class DbCustomContext : DbContext, IDbCustomContext
    {
        private readonly string _tableName;
        private readonly string _connectionString;

        public DbCustomContext(string connectionString, string tableName) : base()
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

        public abstract Task<IList<object>> ToListAsync();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ReplaceService<IModelCacheKeyFactory, CustomModelCacheKeyFactory>();

            optionsBuilder.UseNpgsql(_connectionString);
        }

        public string TableName => _tableName;
    }
}
