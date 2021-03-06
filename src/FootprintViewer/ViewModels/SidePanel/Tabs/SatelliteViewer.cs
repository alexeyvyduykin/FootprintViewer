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
        private readonly IProvider<SatelliteInfo> _provider;
        private readonly ITrackLayerSource _trackLayerSource;
        private readonly ISensorLayerSource _sensorLayerSource;
        private bool _firstLoading = true;

        public SatelliteViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<SatelliteInfo>>();
            _trackLayerSource = dependencyResolver.GetExistingService<ITrackLayerSource>();
            _sensorLayerSource = dependencyResolver.GetExistingService<ISensorLayerSource>();

            Title = "Просмотр спутников";

            ViewerList = ViewerListBuilder.CreateViewerList(_provider);

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => (IFilter<SatelliteInfo>?)null)
                .InvokeCommand(ViewerList.Loading);

            ViewerList.Loading.Subscribe(_ => _firstLoading = false);
        }

        public void UpdateTrack(SatelliteInfo satelliteInfo)
        {
            _trackLayerSource.Update(satelliteInfo);
        }

        public void UpdateStrips(SatelliteInfo satelliteInfo)
        {
            _sensorLayerSource.Update(satelliteInfo);
        }

        [Reactive]
        public IViewerList<SatelliteInfo> ViewerList { get; private set; }
    }
}
