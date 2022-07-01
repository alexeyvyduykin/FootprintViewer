using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public class ProviderViewModel : ReactiveObject
    {
        private bool _isActivated;
        private IEnumerable<string> _availableBuilders = Array.Empty<string>();
        private ProjectFactory? _factory;

        public ProviderViewModel()
        {
            Sources = new List<ISourceInfo>();

            SourceBuilderItems = new List<ISourceBuilderItem>();

            RemoveSource = ReactiveCommand.Create<ISourceInfo>(RemoveSourceImpl);

            AddSource = ReactiveCommand.Create<ISourceInfo, ISourceInfo>(AddSourceImpl);
        }

        public void AddAvailableBuilders(IEnumerable<string> builders, ProjectFactory factory)
        {
            _availableBuilders = builders;
            _factory = factory;
            _isActivated = false;
        }

        public void Activate()
        {
            if (_isActivated || _factory == null)
            {
                return;
            }

            _isActivated = true;

            var items = _factory.CreateSourceBuilderItems(_availableBuilders);

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

        [Reactive]
        public List<ISourceInfo> Sources { get; private set; }

        [Reactive]
        public List<ISourceBuilderItem> SourceBuilderItems { get; private set; }
    }
}
