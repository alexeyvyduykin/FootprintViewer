using FootprintViewer.FileSystem;
using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class MapDataSource : IDataSource<MapResource>
    {
        private readonly SolutionFolder _dataFolder;
        private readonly string? _subFolder;
        private readonly string _file;
        private readonly string? _searchPattern;

        public MapDataSource(string file, string folder, string? subFolder = null)
        {
            _subFolder = subFolder;
            _file = file;

            if (file.Contains("*.mbtiles") == true)
            {
                _searchPattern = file;
            }

            _dataFolder = new SolutionFolder(folder);
        }

        public async Task<List<MapResource>> GetValuesAsync(IFilter<MapResource>? filter = null)
        {
            return await Task.Run(() =>
            {
                IEnumerable<string?> paths;

                if (_searchPattern == null)
                {
                    paths = new[] { _dataFolder.GetPath(_file, _subFolder) };
                }
                else
                {
                    paths = _dataFolder.GetPaths(_searchPattern, _subFolder);
                }

                var list = new List<MapResource>();

                foreach (var path in paths)
                {
                    if (string.IsNullOrEmpty(path) == false)
                    {
                        var name = Path.GetFileNameWithoutExtension(path);

                        list.Add(new MapResource(name, path));
                    }
                }

                return list;
            });
        }
    }
}
