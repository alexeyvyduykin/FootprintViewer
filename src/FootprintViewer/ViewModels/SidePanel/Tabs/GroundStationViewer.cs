﻿using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class GroundStationViewer : SidePanelTab
    {
        private readonly IProvider<GroundStation> _provider;
        private readonly IGroundStationLayerSource _groundStationLayerSource;
        private bool _firstLoading = true;

        public GroundStationViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<GroundStation>>();
            _groundStationLayerSource = dependencyResolver.GetExistingService<IGroundStationLayerSource>();

            Title = "Просмотр наземных станций";

            //ViewerList = ViewerListBuilder.CreateViewerList(_provider, s => new GroundStationInfo(s), s => new GroundStation());

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => (IFilter<GroundStationInfo>?)null)
                .InvokeCommand(ViewerList.Loading);

            ViewerList.Loading.Subscribe(_ => _firstLoading = false);
        }

        public void Update(GroundStationInfo groundStationInfo)
        {
            var name = groundStationInfo.Name;
            var center = new NetTopologySuite.Geometries.Point(groundStationInfo.Center);
            var angles = groundStationInfo.GetAngles();
            var isShow = groundStationInfo.IsShow;

            _groundStationLayerSource.Update(name, center, angles, isShow);
        }

        public void Change(GroundStationInfo groundStationInfo)
        {
            var name = groundStationInfo.Name;
            var center = new NetTopologySuite.Geometries.Point(groundStationInfo.Center);
            var angles = groundStationInfo.GetAngles();
            var isShow = groundStationInfo.IsShow;

            _groundStationLayerSource.Change(name, center, angles, isShow);
        }

        [Reactive]
        public IViewerList<GroundStationInfo> ViewerList { get; private set; }
    }
}
