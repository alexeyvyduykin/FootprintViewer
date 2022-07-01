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
        private readonly ProjectFactory _factory;

        public ProviderViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            _factory = dependencyResolver.GetExistingService<ProjectFactory>();

            Sources = new List<ISourceInfo>();

            SourceBuilderItems = new List<ISourceBuilderItem>();

            RemoveSource = ReactiveCommand.Create<ISourceInfo>(RemoveSourceImpl);

            AddSource = ReactiveCommand.Create<ISourceInfo, ISourceInfo>(AddSourceImpl);
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

        public ReactiveCommand<ISourceInfo, Unit> RemoveSource { get; }

        public ReactiveCommand<ISourceInfo, ISourceInfo> AddSource { get; }

        private void RemoveSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(Sources);

            temp.Remove(item);

            Sources = new List<ISourceInfo>(temp);
        }

        private ISourceInfo AddSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(Sources);

            temp.Add(item);

            Sources = new List<ISourceInfo>(temp);

            return item;
        }

        public ProviderType Type { get; init; }

        public IEnumerable<string>? AvailableBuilders { get; init; }

        [Reactive]
        public List<ISourceInfo> Sources { get; private set; }

        [Reactive]
        public List<ISourceBuilderItem> SourceBuilderItems { get; private set; }
    }
}
