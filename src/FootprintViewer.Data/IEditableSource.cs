namespace FootprintViewer.Data;

public interface IEditableSource : ISource
{
    Task AddAsync(string key, object value);

    Task RemoveAsync(string key, object value);

    Task EditAsync(string key, string id, object newValue);
}
