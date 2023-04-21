using FootprintViewer.Data.DbContexts;

namespace FootprintViewer.Data.Sources;

public class EditableDatabaseSource : BaseEditableSource
{
    private readonly Func<DbCustomContext> _creator;
    private readonly Func<DbCustomContext, string, object, Task> _editor;

    public EditableDatabaseSource(Func<DbCustomContext> creator, Func<DbCustomContext, string, object, Task> editor)
    {
        _creator = creator;
        _editor = editor;
    }

    public override async Task<IList<object>> GetValuesAsync()
    {
        await using var context = _creator.Invoke();

        return await context.GetValuesAsync();
    }

    public override async Task AddAsync(string key, object value)
    {
        await using var context = _creator.Invoke();

        await context.AddAsync(value);

        await context.SaveChangesAsync();
    }

    public override async Task RemoveAsync(string key, object value)
    {
        await using var context = _creator.Invoke();

        context.Remove(value);

        await context.SaveChangesAsync();
    }

    public override async Task EditAsync(string key, string id, object newValue)
    {
        await using var context = _creator.Invoke();

        await _editor.Invoke(context, id, newValue);
    }
}
