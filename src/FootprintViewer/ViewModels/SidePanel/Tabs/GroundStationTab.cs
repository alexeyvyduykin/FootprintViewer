using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundStationTab : SidePanelTab
    {
        private readonly IGroundStationLayerSource _groundStationLayerSource;
        private readonly IProvider<GroundStation> _provider;
        private readonly SourceList<GroundStationViewModel> _groundStation = new();
        private readonly ReadOnlyObservableCollection<GroundStationViewModel> _items;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;

        public GroundStationTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<GroundStation>>();
            _groundStationLayerSource = dependencyResolver.GetExistingService<IGroundStationLayerSource>();

            Title = "Просмотр наземных станций";

            _groundStation
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            Loading = ReactiveCommand.CreateFromTask(LoadingImpl);

            Delay = ReactiveCommand.CreateFromTask(() => Task.Delay(TimeSpan.FromSeconds(1.0)));

            _isLoading = Delay.IsExecuting.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.IsLoading);

            _provider.Observable.Skip(1).Select(s => Unit.Default).InvokeCommand(Loading);

            this.WhenAnyValue(s => s.IsActive)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(active => active == true)
                .Take(1)
                .Select(_ => Unit.Default)
                .InvokeCommand(Loading);

            this.WhenAnyValue(s => s.IsActive)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(active => active == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(Delay);
        }

        private ReactiveCommand<Unit, Unit> Loading { get; }

        private ReactiveCommand<Unit, Unit> Delay { get; }

        public bool IsLoading => _isLoading.Value;

        private async Task LoadingImpl()
        {
            var list = await _provider.GetValuesAsync(null, s => new GroundStationViewModel(s));

            foreach (var item in list)
            {
                item.UpdateObservable.Subscribe(Update);
                item.ChangeObservable.Subscribe(Change);
            }

            _groundStation.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }

        private void Update(GroundStationViewModel groundStation)
        {
            var name = groundStation.Name;
            var center = new NetTopologySuite.Geometries.Point(groundStation.Center);
            var angles = groundStation.GetAngles();
            var isShow = groundStation.IsShow;

            _groundStationLayerSource.Update(name, center, angles, isShow);
        }

        private void Change(GroundStationViewModel groundStation)
        {
            var name = groundStation.Name;
            var center = new NetTopologySuite.Geometries.Point(groundStation.Center);
            var angles = groundStation.GetAngles();
            var isShow = groundStation.IsShow;

            _groundStationLayerSource.Change(name, center, angles, isShow);
        }

        public ReadOnlyObservableCollection<GroundStationViewModel> Items => _items;
    }
}
