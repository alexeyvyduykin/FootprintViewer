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
    public class SatelliteViewer : SidePanelTab
    {
        private readonly IProvider<Satellite> _provider;
        private readonly ITrackLayerSource _trackLayerSource;
        private readonly ISensorLayerSource _sensorLayerSource;
        private bool _firstLoading = true;

        public SatelliteViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<Satellite>>();
            _trackLayerSource = dependencyResolver.GetExistingService<ITrackLayerSource>();
            _sensorLayerSource = dependencyResolver.GetExistingService<ISensorLayerSource>();
            var viewModelFactory = dependencyResolver.GetExistingService<ViewModelFactory>();

            Title = "Просмотр спутников";

            ViewerList = viewModelFactory.CreateSatelliteViewerList(_provider);

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => (IFilter<SatelliteInfo>?)null)
                .InvokeCommand(ViewerList.Loading);

            ViewerList.Loading.Subscribe(_ => _firstLoading = false);
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
