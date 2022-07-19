using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace FootprintViewer.ViewModels
{
    public class ProviderViewModel : ReactiveObject
    {
        private bool _isActivated;
        private readonly ViewModelFactory _factory;

        public ProviderViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            _factory = dependencyResolver.GetExistingService<ViewModelFactory>();

            Sources = new List<ISourceViewModel>();

            SourceBuilderItems = new List<ISourceBuilderItem>();

            RemoveSource = ReactiveCommand.Create<ISourceViewModel>(RemoveSourceImpl);

            AddSource = ReactiveCommand.Create<ISourceViewModel, ISourceViewModel>(AddSourceImpl);
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

        public ReactiveCommand<ISourceViewModel, ISourceViewModel> AddSource { get; }

        private void RemoveSourceImpl(ISourceViewModel item)
        {
            var temp = new List<ISourceViewModel>(Sources);

            temp.Remove(item);

            Sources = new List<ISourceViewModel>(temp);
        }

        private ISourceViewModel AddSourceImpl(ISourceViewModel item)
        {
            var temp = new List<ISourceViewModel>(Sources);

            temp.Add(item);

            Sources = new List<ISourceViewModel>(temp);

            return item;
        }

        public ProviderType Type { get; init; }

        public IEnumerable<string>? AvailableBuilders { get; init; }

        [Reactive]
        public List<ISourceViewModel> Sources { get; private set; }

        [Reactive]
        public List<ISourceBuilderItem> SourceBuilderItems { get; private set; }
    }
}
