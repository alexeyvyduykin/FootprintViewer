using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
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
        private readonly IGroundStationLayerSource _groundStationLayerSource;
        private bool _first = true;

        public GroundStationViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<GroundStationProvider>();
            _groundStationLayerSource = dependencyResolver.GetExistingService<IGroundStationLayerSource>();

            Title = "Просмотр наземных станций";

            Loading = ReactiveCommand.CreateFromTask(LoadingAsync);

            this.WhenAnyValue(s => s.IsActive).Where(active => active == true && _first == true).Select(_ => Unit.Default).InvokeCommand(Loading);

            Loading.Select(s => Convert(s)).ToPropertyEx(this, x => x.GroundStationInfos, scheduler: RxApp.MainThreadScheduler);

            Loading.Subscribe(_ => _first = false);
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

        [ObservableAsProperty]
        public List<GroundStationInfo> GroundStationInfos { get; } = new List<GroundStationInfo>();
    }
}
