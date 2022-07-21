using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class MapDataManager : BaseDataManager<MapResource, IFolderSource>
    {
        protected override async Task<List<MapResource>> GetNativeValuesAsync(IFolderSource dataSource, IFilter<MapResource>? filter)
        {
            return await Task.Run(() =>
            {
                var list = new List<MapResource>();

                if (dataSource.Directory != null && dataSource.SearchPattern != null)
                {
                    var paths = Directory.GetFiles(dataSource.Directory, dataSource.SearchPattern).Select(Path.GetFullPath);

                    foreach (var path in paths)
                    {
                        if (string.IsNullOrEmpty(path) == false)
                        {
                            var name = Path.GetFileNameWithoutExtension(path);

                            list.Add(new MapResource(name, path));
                        }
                    }
                }

                return list;
            });
        }

        protected override Task<List<T>> GetValuesAsync<T>(IFolderSource dataSource, IFilter<T>? filter, System.Func<MapResource, T> converter) => throw new System.NotImplementedException();
    }
}
