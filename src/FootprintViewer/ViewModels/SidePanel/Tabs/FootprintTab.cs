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
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class FootprintTab : SidePanelTab
    {
        private readonly SourceList<FootprintViewModel> _footprints = new();
        private readonly ReadOnlyObservableCollection<FootprintViewModel> _items;
        private readonly IProvider<Footprint> _provider;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        private FootprintViewModel? _prevSelectedItem;

        public FootprintTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<Footprint>>();

            Filter = new FootprintTabFilter(dependencyResolver);

            Title = "Просмотр рабочей программы";

            _footprints
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Filter(Filter.FilterObservable)
                .Bind(out _items)
                .Subscribe();

            Loading = ReactiveCommand.CreateFromTask(LoadingImpl);

            Delay = ReactiveCommand.CreateFromTask(() => Task.Delay(TimeSpan.FromSeconds(1.5)));

            _isLoading = Delay.IsExecuting
                              .ObserveOn(RxApp.MainThreadScheduler)
                              .ToProperty(this, x => x.IsLoading);

            this.WhenAnyValue(s => s.IsActive)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(active => active == true)
                //.Take(1)
                .Select(_ => Unit.Default)
                .InvokeCommand(Loading);

            this.WhenAnyValue(s => s.IsActive)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(active => active == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(Delay);

            Select = ReactiveCommand.Create<FootprintViewModel, FootprintViewModel>(s => s);

            Unselect = ReactiveCommand.Create<FootprintViewModel, FootprintViewModel>(s => s);

            ClickOnItem = ReactiveCommand.Create<FootprintViewModel, FootprintViewModel>(ClickOnItemImpl);
        }

        public ReactiveCommand<FootprintViewModel, FootprintViewModel> Select { get; }

        public ReactiveCommand<FootprintViewModel, FootprintViewModel> Unselect { get; }

        public ReactiveCommand<FootprintViewModel, FootprintViewModel> ClickOnItem { get; }

        public ReactiveCommand<Unit, Unit> Loading { get; }

        private ReactiveCommand<Unit, Unit> Delay { get; }

        public bool IsLoading => _isLoading.Value;

        private async Task LoadingImpl()
        {
            var list = await _provider.GetValuesAsync(null, s => new FootprintViewModel(s));

            _footprints.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }

        private FootprintViewModel ClickOnItemImpl(FootprintViewModel item)
        {
            if (_prevSelectedItem != null && _prevSelectedItem.Name.Equals(item.Name) == false)
            {
                if (_prevSelectedItem.IsShowInfo == true)
                {
                    _prevSelectedItem.IsShowInfo = false;
                }
            }

            item.IsShowInfo = !item.IsShowInfo;

            if (item.IsShowInfo == true)
            {
                Select.Execute(item).Subscribe();
            }
            else
            {
                Unselect.Execute(item).Subscribe();
            }

            _prevSelectedItem = item;

            return item;
        }

        public void SelectFootprintInfo(string name)
        {
            IObservableList<FootprintViewModel> readonlyFootprints = _footprints.AsObservableList();

            var item = readonlyFootprints.Items.Where(s => s.Name.Equals(name)).FirstOrDefault();

            if (item != null)
            {
                ClickOnItemImpl(item);
            }
        }

        public FootprintViewModel? GetFootprintViewModel(string name)
        {
            IObservableList<FootprintViewModel> readonlyFootprints = _footprints.AsObservableList();

            return readonlyFootprints.Items.Where(s => s.Name.Equals(name)).FirstOrDefault();
        }

        public async Task<List<FootprintViewModel>> GetFootprintViewModelsAsync(string name)
        {
            return await _provider.GetValuesAsync(new NameFilter<FootprintViewModel>(new[] { name }), s => new FootprintViewModel(s));
        }

        public IFilter<FootprintViewModel> Filter { get; }

        public ReadOnlyObservableCollection<FootprintViewModel> Items => _items;

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;
    }
}
