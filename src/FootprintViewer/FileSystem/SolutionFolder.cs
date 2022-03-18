using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FootprintViewer.FileSystem
{
    public class SolutionFolder
    {
        private readonly string _solutionFolderName;
        private readonly string? _path;

#if DEBUG
        private readonly string _separator = @"..\..\..\..\..\";
#else
        private readonly string _separator = "";
#endif

        public SolutionFolder(string name)
        {
            _solutionFolderName = name;
            _path = Directory.GetCurrentDirectory();// AppDomain.CurrentDomain.BaseDirectory;

            if (_path != null)
            {
                _path = Path.GetFullPath(Path.Combine(_path, _separator));
                _path = Path.Combine(_path, _solutionFolderName);
            }
        }

        public string? GetPath(string fileName, string? subFolder = null)
        {
            if (_path != null)
            {
                var path = string.IsNullOrEmpty(subFolder) ? _path : Path.Combine(_path, subFolder);

                CreateIfMissing(path);

                return Path.Combine(path, fileName);
            }

            return null;
        }

        public IEnumerable<string> GetPaths(string searchPattern, string? subFolder = null)
        {
            if (_path != null)
            {
                var path = string.IsNullOrEmpty(subFolder) ? _path : Path.Combine(_path, subFolder);

                CreateIfMissing(path);

                return Directory.GetFiles(path, searchPattern).Select(Path.GetFullPath);
            }

            return new List<string>();
        }

        private void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);

            if (folderExists == false)
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
