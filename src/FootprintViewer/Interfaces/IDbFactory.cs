using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;

namespace FootprintViewer;

public interface IDbFactory
{
    ISource CreateSource(DbKeys key, string connectionString, string tableName);

    ISource CreateSource(DbKeys key, string jsonFilePath);

    DbCustomContext CreateContext(DbKeys key, string connectionString, string tableName);
}
