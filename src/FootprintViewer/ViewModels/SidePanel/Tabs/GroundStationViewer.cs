using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class GroundStationList : ViewerList<GroundStationInfo>
    {
        public GroundStationList(GroundStationProvider provider) : base(provider)
        {

        }
    }

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

            ViewerList = new GroundStationList(_provider);

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _first == true)
                .Select(_ => (IFilter<GroundStationInfo>?)null)
                .InvokeCommand(ViewerList.Loading);

            ViewerList.Loading.Subscribe(_ => _first = false);
        }

        public void Update(GroundStationInfo groundStationInfo)
        {
            _groundStationLayerSource.Update(groundStationInfo);
        }

        public void Change(GroundStationInfo groundStationInfo)
        {
            _groundStationLayerSource.Change(groundStationInfo);
        }

        [Reactive]
        public IViewerList<GroundStationInfo> ViewerList { get; private set; }
    }
}
