using DynamicData;
using FootprintViewer.Data;
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
    public class ProviderViewModel : ReactiveObject
    {
        private bool _isActivated;
        private readonly ViewModelFactory _factory;
        private readonly ReadOnlyObservableCollection<ISourceViewModel> _sources;

        public ProviderViewModel(IProvider provider, IReadonlyDependencyResolver dependencyResolver)
        {
            _factory = dependencyResolver.GetExistingService<ViewModelFactory>();

            SourceBuilderItems = new List<ISourceBuilderItem>();

            provider.Sources
                .Connect()
                .Transform(s => s.ToViewModel())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _sources)
                .Subscribe();

            RemoveSource = ReactiveCommand.Create<ISourceViewModel>(s => provider.Sources.Remove(s.ToModel()));

            AddSource = ReactiveCommand.Create<ISourceViewModel>(s => provider.Sources.Add(s.ToModel()));
        }

        public void Activate()
        {
            if (_isActivated || AvailableBuilders == null)
            {
                return;
            }

            _isActivated = true;

            var items = _factory.CreateSourceBuilderItems(AvailableBuilders);

            SourceBuilderItems = new List<ISourceBuilderItem>(items);

            foreach (var item in SourceBuilderItems)
            {
                item.Build.Where(s => s != null).Select(s => s!).InvokeCommand(AddSource);
            }
        }

        public ReactiveCommand<ISourceViewModel, Unit> RemoveSource { get; }

        public ReactiveCommand<ISourceViewModel, Unit> AddSource { get; }

        public ProviderType Type { get; init; }

        public IEnumerable<string>? AvailableBuilders { get; init; }

        public ReadOnlyObservableCollection<ISourceViewModel> Sources => _sources;

        [Reactive]
        public List<ISourceBuilderItem> SourceBuilderItems { get; private set; }
    }
}
