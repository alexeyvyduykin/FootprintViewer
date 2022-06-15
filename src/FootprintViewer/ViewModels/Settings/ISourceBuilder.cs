﻿using ReactiveUI;
using System.Windows.Input;

namespace FootprintViewer.ViewModels
{
    public interface ISourceBuilder
    {
        string Name { get; }

        ICommand Build { get; set; }
    }

    public class DatabaseSourceBuilder : ReactiveObject, ISourceBuilder
    {
        private readonly AppSettings? _settings;

        public DatabaseSourceBuilder()
        {
            Build = ReactiveCommand.Create<ISourceInfo>(() => new DatabaseSourceInfo()
            {
                Version = _settings?.LastDatabaseSource?.Version,
                Host = _settings?.LastDatabaseSource?.Host,
                Port = (_settings?.LastDatabaseSource != null) ? _settings.LastDatabaseSource.Port : 0,
                Database = _settings?.LastDatabaseSource?.Database,
                Username = _settings?.LastDatabaseSource?.Username,
                Password = _settings?.LastDatabaseSource?.Password,
                Table = _settings?.LastDatabaseSource?.Table,
            });
        }

        public DatabaseSourceBuilder(AppSettings settings) : this()
        {
            _settings = settings;
        }

        public string Name => "Add database";

        public ICommand Build { get; set; }
    }

    public class RandomSourceBuilder : ReactiveObject, ISourceBuilder
    {
        public RandomSourceBuilder(string name) : base()
        {
            Build = ReactiveCommand.Create<ISourceInfo>(() => new RandomSourceInfo(name));
        }

        public string Name => "Add random";

        public ICommand Build { get; set; }
    }

    public class FileSourceBuilder : ReactiveObject, ISourceBuilder
    {
        public FileSourceBuilder(string filterName, string filterExtension) : base()
        {
            Build = ReactiveCommand.Create<ISourceInfo>(() => new FileSourceInfo() { FilterName = filterName, FilterExtension = filterExtension });
        }

        public string Name => "Add file";

        public ICommand Build { get; set; }
    }

    public class FolderSourceBuilder : ReactiveObject, ISourceBuilder
    {
        public FolderSourceBuilder(string searchPattern) : base()
        {
            Build = ReactiveCommand.Create<ISourceInfo>(() => new FolderSourceInfo() { SearchPattern = searchPattern });
        }

        public string Name => "Add folder";

        public ICommand Build { get; set; }
    }
}
