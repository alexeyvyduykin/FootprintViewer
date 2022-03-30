using FootprintViewer.Data;
using FootprintViewer.Layers;
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
        private readonly SatelliteProvider _provider;
        private readonly ObservableAsPropertyHelper<List<SatelliteInfo>> _satellites;
        private readonly ITrackLayerSource _trackLayerSource;
        private readonly ISensorLayerSource _sensorLayerSource;

        public SatelliteViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<SatelliteProvider>();
            _trackLayerSource = dependencyResolver.GetExistingService<ITrackLayerSource>();
            _sensorLayerSource = dependencyResolver.GetExistingService<ISensorLayerSource>();

            Title = "Просмотр спутников";

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
            _trackLayerSource.Update(satelliteInfo);
        }

        public void UpdateStrips(SatelliteInfo satelliteInfo)
        {
            _sensorLayerSource.Update(satelliteInfo);
        }

        public List<SatelliteInfo> SatelliteInfos => _satellites.Value;
    }
}
