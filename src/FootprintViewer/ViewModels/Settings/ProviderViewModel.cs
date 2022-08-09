using DynamicData;
using FootprintViewer.AppStates;
using FootprintViewer.Data;
using FootprintViewer.Data.Sources;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class ProviderViewModel : ReactiveObject, ISuspendableState<ProviderState>
    {
        private bool _isActivated;
        private readonly ViewModelFactory _factory;
        private readonly ReadOnlyObservableCollection<ISourceViewModel> _sources;
        private ProviderState? _state;
        private readonly IProvider _provider;

        public ProviderViewModel(IProvider provider, IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = provider;
            _factory = dependencyResolver.GetExistingService<ViewModelFactory>();

            SourceBuilderItems = new List<ISourceBuilderItem>();

            provider.Observable
                .Transform(s => s.ToViewModel())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _sources)
                .Subscribe();

            RemoveSource = ReactiveCommand.Create<ISourceViewModel>(RemoveSourceImpl);

            AddSource = ReactiveCommand.Create<ISourceViewModel>(AddSourceImpl);
        }

        public void Activate()
        {
            if (_isActivated || AvailableBuilders == null)
            {
                return;
            }

            _isActivated = true;

            var items = _factory.CreateSourceBuilderItems(AvailableBuilders, Builder);

            SourceBuilderItems = new List<ISourceBuilderItem>(items);

            foreach (var item in SourceBuilderItems)
            {
                item.Build.Where(s => s != null).Select(s => s!).InvokeCommand(AddSource);
            }
        }

        public void LoadState(ProviderState state)
        {
            _state = state;

            var res = state.Sources.Values.Select(s => ToDataSource(s)).ToList();

            _provider.AddSources(res);
        }

        private void AddSourceImpl(ISourceViewModel source)
        {
            _provider.AddSources(new[] { source.ToModel() });

            if (_state != null)
            {
                _state.Add(ToSourceState(source));
            }
        }

        private void RemoveSourceImpl(ISourceViewModel source)
        {
            _provider.RemoveSources(new[] { source.ToModel() });

            if (_state != null)
            {
                _state.Remove(ToSourceState(source));
            }
        }

        private static IDataSource ToDataSource(ISourceState state)
        {
            if (state is DatabaseSourceState databaseSourceState)
            {
                return new DatabaseSource()
                {
                    Database = databaseSourceState.Database!,
                    Host = databaseSourceState.Host!,
                    Version = databaseSourceState.Version!,
                    Port = databaseSourceState.Port,
                    Username = databaseSourceState.Username!,
                    Password = databaseSourceState.Password!,
                    Table = databaseSourceState.Table!,
                };
            }
            else if (state is FileSourceState fileSourceState)
            {
                return new FileSource()
                {
                    Path = fileSourceState.Path!,
                };
            }
            else if (state is FolderSourceState folderSourceState)
            {
                return new FolderSource()
                {
                    Directory = folderSourceState.Directory!,
                    SearchPattern = folderSourceState.SearchPattern!,
                };
            }
            else if (state is RandomSourceState randomSourceState)
            {
                return new RandomSource()
                {
                    GenerateCount = randomSourceState.GenerateCount,
                    Name = randomSourceState.Name!,
                };
            }
            else
            {
                throw new Exception();
            }
        }

        private static ISourceState ToSourceState(ISourceViewModel source)
        {
            if (source is DatabaseSourceViewModel databaseSource)
            {
                return new DatabaseSourceState()
                {
                    Name = databaseSource.Name,
                    Database = databaseSource.Database,
                    Host = databaseSource.Host,
                    Version = databaseSource.Version,
                    Port = databaseSource.Port,
                    Username = databaseSource.Username,
                    Password = databaseSource.Password,
                    Table = databaseSource.Table,
                };
            }
            else if (source is FileSourceViewModel fileSource)
            {
                return new FileSourceState()
                {
                    Name = fileSource.Name,
                    Path = fileSource.Path,
                };
            }
            else if (source is FolderSourceViewModel folderSource)
            {
                return new FolderSourceState()
                {
                    Name = folderSource.Name,
                    Directory = folderSource.Directory,
                    SearchPattern = folderSource.SearchPattern,
                };
            }
            else if (source is RandomSourceViewModel randomSource)
            {
                return new RandomSourceState()
                {
                    GenerateCount = randomSource.GenerateCount,
                    Name = randomSource.Name,
                };
            }
            else
            {
                throw new Exception();
            }
        }

        public ReactiveCommand<ISourceViewModel, Unit> RemoveSource { get; }

        public ReactiveCommand<ISourceViewModel, Unit> AddSource { get; }

        public ProviderType Type { get; init; }

        public IEnumerable<string>? AvailableBuilders { get; init; }

        public Func<SourceType, ISourceViewModel>? Builder { get; init; }

        public ReadOnlyObservableCollection<ISourceViewModel> Sources => _sources;

        [Reactive]
        public List<ISourceBuilderItem> SourceBuilderItems { get; private set; }
    }
}
