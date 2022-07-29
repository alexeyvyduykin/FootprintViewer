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
    public class SatelliteTab : SidePanelTab
    {
        private readonly ITrackLayerSource _trackLayerSource;
        private readonly ISensorLayerSource _sensorLayerSource;
        private readonly IProvider<Satellite> _provider;
        private readonly SourceList<SatelliteViewModel> _satellites = new();
        private readonly ReadOnlyObservableCollection<SatelliteViewModel> _items;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;

        public SatelliteTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<Satellite>>();
            _trackLayerSource = dependencyResolver.GetExistingService<ITrackLayerSource>();
            _sensorLayerSource = dependencyResolver.GetExistingService<ISensorLayerSource>();

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
            var list = await _provider.GetValuesAsync(null, s => new SatelliteViewModel(s));

            foreach (var item in list)
            {
                item.TrackObservable.Subscribe(UpdateTrack);
                item.StripsObservable.Subscribe(UpdateStrips);
            }

            _satellites.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }

        private void UpdateTrack(SatelliteViewModel satelliteInfo)
        {
            var name = satelliteInfo.Name;
            var node = satelliteInfo.CurrentNode;
            var isShow = satelliteInfo.IsShow && satelliteInfo.IsTrack;

            _trackLayerSource.Update(name, node, isShow);
        }

        private void UpdateStrips(SatelliteViewModel satelliteInfo)
        {
            var name = satelliteInfo.Name;
            var node = satelliteInfo.CurrentNode;
            var isShow1 = satelliteInfo.IsShow && satelliteInfo.IsLeftStrip;
            var isShow2 = satelliteInfo.IsShow && satelliteInfo.IsRightStrip;

            _sensorLayerSource.Update(name, node, isShow1, isShow2);
        }

        public ReadOnlyObservableCollection<SatelliteViewModel> Items => _items;
    }
}
