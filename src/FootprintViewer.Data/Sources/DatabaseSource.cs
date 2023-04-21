using FootprintViewer.Data.DbContexts;

namespace FootprintViewer.Data.Sources;

public class DatabaseSource : BaseSource
{
    private readonly Func<DbCustomContext> _creator;

    public DatabaseSource(Func<DbCustomContext> creator)
    {
        _creator = creator;
    }

    public override async Task<IList<object>> GetValuesAsync()
    {
        await using var context = _creator.Invoke();

        return await context.GetValuesAsync();
    }
}
