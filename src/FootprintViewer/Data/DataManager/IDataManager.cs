using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager;

public interface IDataManager
{
    void RegisterSource(string key, ISource source);

    void UnregisterSource(string key, ISource source);

    IReadOnlyList<ISource> GetSources(string key);

    Task<IList<T>> GetDataAsync<T>(string key, bool caching = true);
}
