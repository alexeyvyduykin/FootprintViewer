using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Services;
using System.Collections.Generic;

namespace FootprintViewer.Extensions;

public static class LocalStorageServiceExtensions
{
    public static IReadOnlyList<ISource> GetSources(this ILocalStorageService localStorage, DbKeys key)
    {
        return localStorage.GetSources(key.ToString());
    }
}
