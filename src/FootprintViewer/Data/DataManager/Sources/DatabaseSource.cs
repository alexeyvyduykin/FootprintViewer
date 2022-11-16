using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager.Sources;

public class DatabaseSource : BaseSource
{
    private readonly string _key;
    private readonly string _connectionString;
    private readonly string _tableName;

    public DatabaseSource(string key, string connectionString, string tableName)
    {
        _key = key;
        _connectionString = connectionString;
        _tableName = tableName;
    }

    public string Key => _key;

    public string ConnectionString => _connectionString;

    public string TableName => _tableName;

    public override async Task<IList<object>> GetValuesAsync()
    {
        await using var context = DbHelper.CreateDatabaseSource(_key, _tableName).Invoke(_connectionString);

        return await context.GetTable().ToListAsync();
    }
}
