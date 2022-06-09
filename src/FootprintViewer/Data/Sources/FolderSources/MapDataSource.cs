using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class MapDataSource : IDataSource<MapResource>
    {
        private readonly string? _directory;
        private readonly string? _searchPattern;

        public MapDataSource(string? directory, string? searchPattern)
        {
            _directory = directory;
            _searchPattern = searchPattern;
        }

        public async Task<List<MapResource>> GetValuesAsync(IFilter<MapResource>? filter = null)
        {
            return await Task.Run(() =>
            {
                var list = new List<MapResource>();

                if (_directory != null && _searchPattern != null)
                {
                    var paths = Directory.GetFiles(_directory, _searchPattern).Select(Path.GetFullPath);

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
    }
}
