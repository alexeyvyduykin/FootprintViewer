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
    public class SatelliteList : ViewerList<SatelliteInfo>
    {
        public SatelliteList(SatelliteProvider provider) : base(provider)
        {

        }
    }

    public class SatelliteViewer : SidePanelTab
    {
        private readonly SatelliteProvider _provider;
        private readonly ITrackLayerSource _trackLayerSource;
        private readonly ISensorLayerSource _sensorLayerSource;
        private bool _first = true;

        public SatelliteViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<SatelliteProvider>();
            _trackLayerSource = dependencyResolver.GetExistingService<ITrackLayerSource>();
            _sensorLayerSource = dependencyResolver.GetExistingService<ISensorLayerSource>();

            Title = "Просмотр спутников";

            ViewerList = new SatelliteList(_provider);

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _first == true)
                .Select(_ => (IFilter<SatelliteInfo>?)null)
                .InvokeCommand(ViewerList.Loading);

            ViewerList.Loading.Subscribe(_ => _first = false);
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
        public ViewerList<SatelliteInfo> ViewerList { get; private set; }
    }
}
