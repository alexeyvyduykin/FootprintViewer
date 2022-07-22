using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class GroundStationTab : SidePanelTab
    {
        private readonly IGroundStationLayerSource _groundStationLayerSource;

        public GroundStationTab(IReadonlyDependencyResolver dependencyResolver)
        {
            var provider = dependencyResolver.GetExistingService<IProvider<GroundStation>>();
            _groundStationLayerSource = dependencyResolver.GetExistingService<IGroundStationLayerSource>();
            var viewModelFactory = dependencyResolver.GetExistingService<ViewModelFactory>();

            Title = "Просмотр наземных станций";

            ViewerList = viewModelFactory.CreateGroundStationViewerList(provider);

            provider.Observable.Skip(1).Select(s => (IFilter<GroundStationViewModel>?)null).InvokeCommand(ViewerList.Loading);

            // First loading

            // TODO: with Take(1) not call
            this.WhenAnyValue(s => s.IsActive)
                .Take(2)
                .Where(active => active == true)
                .Select(_ => (IFilter<GroundStationViewModel>?)null)
                .InvokeCommand(ViewerList.Loading);
        }

        public void Update(GroundStationViewModel groundStationInfo)
        {
            var name = groundStationInfo.Name;
            var center = new NetTopologySuite.Geometries.Point(groundStationInfo.Center);
            var angles = groundStationInfo.GetAngles();
            var isShow = groundStationInfo.IsShow;

            _groundStationLayerSource.Update(name, center, angles, isShow);
        }

        public void Change(GroundStationViewModel groundStationInfo)
        {
            var name = groundStationInfo.Name;
            var center = new NetTopologySuite.Geometries.Point(groundStationInfo.Center);
            var angles = groundStationInfo.GetAngles();
            var isShow = groundStationInfo.IsShow;

            _groundStationLayerSource.Change(name, center, angles, isShow);
        }

        [Reactive]
        public IViewerList<GroundStationViewModel> ViewerList { get; private set; }
    }
}
