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
    public class GroundStationTab : SidePanelTab
    {
        private readonly IProvider<GroundStation> _provider;
        private readonly IGroundStationLayerSource _groundStationLayerSource;
        private bool _firstLoading = true;

        public GroundStationTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<GroundStation>>();
            _groundStationLayerSource = dependencyResolver.GetExistingService<IGroundStationLayerSource>();
            var viewModelFactory = dependencyResolver.GetExistingService<ViewModelFactory>();

            Title = "Просмотр наземных станций";

            ViewerList = viewModelFactory.CreateGroundStationViewerList(_provider);

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => (IFilter<GroundStationViewModel>?)null)
                .InvokeCommand(ViewerList.Loading);

            ViewerList.Loading.Subscribe(_ => _firstLoading = false);
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
