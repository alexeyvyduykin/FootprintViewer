using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui;
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
    public class SatelliteTab : SidePanelTab
    {
        private readonly TrackLayer? _trackLayer;
        private readonly SensorLayer? _sensorLayer;
        private readonly IProvider<Satellite> _provider;
        private readonly SourceList<SatelliteViewModel> _satellites = new();
        private readonly ReadOnlyObservableCollection<SatelliteViewModel> _items;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;

        public SatelliteTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<Satellite>>();
            var map = (Map)dependencyResolver.GetExistingService<IMap>();
            _sensorLayer = map.GetLayer<SensorLayer>(LayerType.Sensor);
            _trackLayer = map.GetLayer<TrackLayer>(LayerType.Track);

            Title = "Просмотр спутников";

            _satellites
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            Loading = ReactiveCommand.CreateFromTask(LoadingImpl);

            Delay = ReactiveCommand.CreateFromTask(() => Task.Delay(TimeSpan.FromSeconds(1.0)));

            _isLoading = Delay.IsExecuting
                  .ObserveOn(RxApp.MainThreadScheduler)
                  .ToProperty(this, x => x.IsLoading);

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

        public ReactiveCommand<Unit, Unit> Loading { get; }

        private ReactiveCommand<Unit, Unit> Delay { get; }

        public bool IsLoading => _isLoading.Value;

        private async Task LoadingImpl()
        {
            var list = await _provider.GetValuesAsync(null, s => new SatelliteViewModel(s));

            foreach (var item in list)
            {
                item.TrackObservable.Subscribe(s => _trackLayer?.Update(s));
                item.StripsObservable.Subscribe(s => _sensorLayer?.Update(s));
            }

            _satellites.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }

        public ReadOnlyObservableCollection<SatelliteViewModel> Items => _items;
    }
}
