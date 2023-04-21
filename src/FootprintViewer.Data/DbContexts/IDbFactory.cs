namespace FootprintViewer.Data.DbContexts;

public interface IDbFactory
{
    ISource CreateSource(DbKeys key, string connectionString, string tableName);

    DbCustomContext CreateContext(DbKeys key, string connectionString, string tableName);
}
