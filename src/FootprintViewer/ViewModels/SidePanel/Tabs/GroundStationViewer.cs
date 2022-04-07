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
    public class GroundStationViewer : SidePanelTab
    {
        private readonly GroundStationProvider _provider;
        private readonly ObservableAsPropertyHelper<List<GroundStationInfo>> _groundStations;
        private readonly IGroundStationLayerSource _groundStationLayerSource;

        public GroundStationViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<GroundStationProvider>();
            _groundStationLayerSource = dependencyResolver.GetExistingService<IGroundStationLayerSource>();

            Title = "Просмотр наземных станций";

            Loading = ReactiveCommand.CreateFromTask(LoadingAsync);

            this.WhenAnyValue(s => s.IsActive).Where(active => active == true).Select(_ => Unit.Default).InvokeCommand(Loading);

            _groundStations = Loading.Select(s => Convert(s)).ToProperty(this, x => x.GroundStationInfos, scheduler: RxApp.MainThreadScheduler);
        }

        protected ReactiveCommand<Unit, List<GroundStation>> Loading { get; }

        private async Task<List<GroundStation>> LoadingAsync() => await _provider.GetGroundStationsAsync();

        private static List<GroundStationInfo> Convert(List<GroundStation> groundStations)
        {
            if (groundStations == null)
            {
                return new List<GroundStationInfo>();
            }

            return groundStations.Select(s => new GroundStationInfo(s)).ToList();
        }

        public void Update(GroundStationInfo groundStationInfo)
        {
            _groundStationLayerSource.Update(groundStationInfo);
        }

        public void Change(GroundStationInfo groundStationInfo)
        {
            _groundStationLayerSource.Change(groundStationInfo);
        }

        public List<GroundStationInfo> GroundStationInfos => _groundStations.Value;
    }
}
