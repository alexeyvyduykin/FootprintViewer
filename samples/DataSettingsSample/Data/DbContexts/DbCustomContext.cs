using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
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

    public class DbCustomContext : DbContext
    {
        private readonly string _tableName;

        public DbCustomContext(string tableName, DbContextOptions options) : base(options)
        {
            _tableName = tableName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ReplaceService<IModelCacheKeyFactory, CustomModelCacheKeyFactory>();
        }

        public string TableName => _tableName;
    }
}
