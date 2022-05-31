using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FootprintViewer.FileSystem
{
    public class SolutionFolder
    {
        private static readonly string _separator = @"..\..\..\..\..\";
        private readonly string _name;
        private readonly string _folderDirectory;
        private readonly string _solutionDirectory;

        public SolutionFolder(string name)
        {
            _name = name;

            _solutionDirectory = GetSolutionDirectory();

            _folderDirectory = Path.Combine(_solutionDirectory, name);
        }

        public static string SourceFolder => "src";

        public string DataFolder => _name;

        public string FolderDirectory
        {
            get
            {
                // TODO: FolderDirectory empty if name not exist, refactoring this (DesignTime throw exception if in ctor)
                CreateIfMissing(_folderDirectory);

                return _folderDirectory;
            }
        }

        public string SolutionDirectory => _solutionDirectory;

        private static string GetSolutionDirectory()
        {
            var path = Directory.GetCurrentDirectory();

            if (path.Contains(@"\\bin\\Debug\\") || path.Contains(@"\\bin\\Release\\") ||
                path.Contains(@"\bin\Debug\") || path.Contains(@"\bin\Release\"))
            {
                path = Path.GetFullPath(Path.Combine(path, _separator));
            }

            return path;
        }

        private static string GetProjectDirectory()
        {
            string projectName = string.Empty;

            var assembly = Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                projectName = assembly.GetName().Name ?? string.Empty;
            }

            var root = GetSolutionDirectory();

            return Path.Combine(root, SourceFolder, projectName);
        }

        public static string GetAppSettingsBasePath(string settingsFilename)
        {
            var solutionDir = GetSolutionDirectory();

            if (File.Exists(Path.Combine(solutionDir, settingsFilename)) == true)
            {
                return solutionDir;
            }

            var projectDir = GetProjectDirectory();

            if (File.Exists(Path.Combine(projectDir, settingsFilename)) == true)
            {
                return projectDir;
            }

            throw new Exception($"SolutionDirectory: Base path {projectDir} for file {settingsFilename} not found");
        }

        public string? GetPath(string fileName, string? subFolder = null)
        {
            if (_folderDirectory != null)
            {
                var path = string.IsNullOrEmpty(subFolder) ? _folderDirectory : Path.Combine(_folderDirectory, subFolder);

                CreateIfMissing(path);

                return Path.Combine(path, fileName);
            }

            return null;
        }

        public IEnumerable<string> GetPaths(string searchPattern, string? subFolder = null)
        {
            if (_folderDirectory != null)
            {
                var path = string.IsNullOrEmpty(subFolder) ? _folderDirectory : Path.Combine(_folderDirectory, subFolder);

                CreateIfMissing(path);

                return Directory.GetFiles(path, searchPattern).Select(Path.GetFullPath);
            }

            return new List<string>();
        }

        private static void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);

            if (folderExists == false)
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
