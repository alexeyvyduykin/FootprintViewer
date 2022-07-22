using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class SatelliteViewer : SidePanelTab
    {
        private readonly ITrackLayerSource _trackLayerSource;
        private readonly ISensorLayerSource _sensorLayerSource;

        public SatelliteViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            var provider = dependencyResolver.GetExistingService<IProvider<Satellite>>();
            _trackLayerSource = dependencyResolver.GetExistingService<ITrackLayerSource>();
            _sensorLayerSource = dependencyResolver.GetExistingService<ISensorLayerSource>();
            var viewModelFactory = dependencyResolver.GetExistingService<ViewModelFactory>();

            Title = "Просмотр спутников";

            ViewerList = viewModelFactory.CreateSatelliteViewerList(provider);

            provider.Observable.Skip(1).Select(s => (IFilter<SatelliteInfo>?)null).InvokeCommand(ViewerList.Loading);

            // First loading

            // TODO: with Take(1) not call
            this.WhenAnyValue(s => s.IsActive)
                .Take(2)
                .Where(active => active == true)
                .Select(_ => (IFilter<SatelliteInfo>?)null)
                .InvokeCommand(ViewerList.Loading);
        }

        public void UpdateTrack(SatelliteInfo satelliteInfo)
        {
            var name = satelliteInfo.Name;
            var node = satelliteInfo.CurrentNode;
            var isShow = satelliteInfo.IsShow && satelliteInfo.IsTrack;

            _trackLayerSource.Update(name, node, isShow);
        }

        public void UpdateStrips(SatelliteInfo satelliteInfo)
        {
            var name = satelliteInfo.Name;
            var node = satelliteInfo.CurrentNode;
            var isShow1 = satelliteInfo.IsShow && satelliteInfo.IsLeftStrip;
            var isShow2 = satelliteInfo.IsShow && satelliteInfo.IsRightStrip;

            _sensorLayerSource.Update(name, node, isShow1, isShow2);
        }

        [Reactive]
        public IViewerList<SatelliteInfo> ViewerList { get; private set; }
    }
}
