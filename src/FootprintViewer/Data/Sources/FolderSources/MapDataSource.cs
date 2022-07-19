using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class MapDataSource : BaseFolderSource, IDataSource<MapResource>
    {
        public async Task<List<MapResource>> GetNativeValuesAsync(IFilter<MapResource>? filter)
        {
            return await Task.Run(() =>
            {
                var list = new List<MapResource>();

                if (Directory != null && SearchPattern != null)
                {
                    var paths = System.IO.Directory.GetFiles(Directory, SearchPattern).Select(Path.GetFullPath);

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

        public Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, System.Func<MapResource, T> converter) => throw new System.NotImplementedException();
    }
}
