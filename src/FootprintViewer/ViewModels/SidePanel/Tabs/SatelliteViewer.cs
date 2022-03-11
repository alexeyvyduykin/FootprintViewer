using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class SatelliteViewer : SidePanelTab
    {
        private readonly TrackLayer? _trackLayer;
        private readonly SensorLayer? _sensorLayer;
        private readonly SatelliteProvider _provider;
        private readonly ObservableAsPropertyHelper<List<SatelliteInfo>> _satellites;

        public SatelliteViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            var map = dependencyResolver.GetExistingService<Map>();

            _provider = dependencyResolver.GetExistingService<SatelliteProvider>();

            Title = "Просмотр спутников";

            _trackLayer = map.GetLayer<TrackLayer>(LayerType.Track);

            _sensorLayer = map.GetLayer<SensorLayer>(LayerType.Sensor);

            Loading = ReactiveCommand.CreateFromTask(LoadingAsync);

            this.WhenAnyValue(s => s.IsActive).Where(active => active == true).Select(_ => Unit.Default).InvokeCommand(Loading);

            _satellites = Loading.Select(s => Convert(s)).ToProperty(this, x => x.SatelliteInfos, scheduler: RxApp.MainThreadScheduler);
        }

        private ReactiveCommand<Unit, List<Satellite>> Loading { get; }

        private async Task<List<Satellite>> LoadingAsync() => await _provider.GetSatellitesAsync();

        private List<SatelliteInfo> Convert(List<Satellite> satellites)
        {
            if (satellites == null)
            {
                return new List<SatelliteInfo>();
            }

            return satellites.Select(s => new SatelliteInfo(s)).ToList();
        }

        public void UpdateTrack(SatelliteInfo satelliteInfo)
        {
            _trackLayer?.Update(satelliteInfo);
        }

        public void UpdateStrips(SatelliteInfo satelliteInfo)
        {
            _sensorLayer?.Update(satelliteInfo);
        }

        public List<SatelliteInfo> SatelliteInfos => _satellites.Value;
    }
}
