using FootprintViewer.FileSystem;
using System.Collections.Generic;
using System.IO;

namespace FootprintViewer.Data.Sources
{
    public class MapDataSource : IMapDataSource
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

        public IEnumerable<MapResource> GetMapResources()
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

            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path) == false)
                {
                    var name = Path.GetFileNameWithoutExtension(path);

                    yield return new MapResource(name, path);
                }
            }
        }
    }
}
