namespace FootprintViewer.Data;

public interface IDataManager
{
    void RegisterSource(string key, ISource source);

    void UnregisterSource(string key, ISource source);

    void UnregisterSources(string key);

    IReadOnlyList<ISource> GetSources(string key);

    IReadOnlyList<string> GetKeys();

    IReadOnlyDictionary<string, IReadOnlyList<ISource>> GetSources();

    Task<IList<T>> GetDataAsync<T>(string key, bool caching = true);

    Task<bool> TryAddAsync(string key, object value);

    Task<bool> TryRemoveAsync(string key, object value);

    Task<bool> TryEditAsync(string key, string id, object newValue);
}
