using FootprintViewer.Data.DbContexts;

namespace FootprintViewer.Data;

public static class DataManagerExtensions
{
    public static IReadOnlyList<ISource> GetSources(this IDataManager dataManager, DbKeys key)
    {
        return dataManager.GetSources(key.ToString());
    }
}
