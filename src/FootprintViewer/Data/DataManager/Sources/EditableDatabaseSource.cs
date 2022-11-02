using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager.Sources;

public class EditableDatabaseSource : BaseEditableSource
{
    private readonly string _key;
    private readonly string _connectionString;
    private readonly string _tableName;

    public EditableDatabaseSource(string key, string connectionString, string tableName)
    {
        _key = key;
        _connectionString = connectionString;
        _tableName = tableName;
    }

    public override async Task<IList<object>> GetValuesAsync()
    {
        await using var context = DbHelper.CreateDatabaseSource(_key, _tableName).Invoke(_connectionString);

        return await context.GetTable().ToListAsync();
    }

    public override async Task AddAsync(string key, object value)
    {
        //var options = extns2.BuildDbContextOptions<UserGeometryDbContext>(source);
        //using var context = new UserGeometryDbContext(source.Table, options);

        await using var context = DbHelper.CreateDatabaseSource(_key, _tableName).Invoke(_connectionString);

        await context.AddAsync(value);

        await context.SaveChangesAsync();
    }

    public override async Task RemoveAsync(string key, object value)
    {
        //var options = extns2.BuildDbContextOptions<UserGeometryDbContext>(source);
        //using var context = new UserGeometryDbContext(source.Table, options);

        await using var context = DbHelper.CreateDatabaseSource(_key, _tableName).Invoke(_connectionString);

        context.Remove(value);

        await context.SaveChangesAsync();
    }

    public override async Task EditAsync(string key, string id, object newValue)
    {
        //var options = extns2.BuildDbContextOptions<UserGeometryDbContext>(source);
        //using var context = new UserGeometryDbContext(source.Table, options);
        await using var context = DbHelper.CreateDatabaseSource(_key, _tableName).Invoke(_connectionString);

        await DbHelper.EditAsync(_key, _connectionString, _tableName, id, newValue);
    }
}
