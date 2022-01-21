using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Projection;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class FootprintObserver : SidePanelTab
    {
        private readonly FootprintLayer _footrpintLayer;
        private readonly Map _map;
        private readonly FootprintObserverList _footprintObserverList;
        private readonly PreviewMainContent _updateMainContent;

        public FootprintObserver(IReadonlyDependencyResolver dependencyResolver)
        {
            var map = dependencyResolver.GetExistingService<Map>();

            _map = map;

            Title = "Просмотр рабочей программы";

            Name = "FootprintViewer";

            _footrpintLayer = map.GetLayer<FootprintLayer>(LayerType.Footprint);

            Filter = new FootprintObserverFilter(dependencyResolver);

            _updateMainContent = new PreviewMainContent("Загрузка...");

            _footprintObserverList = new FootprintObserverList(dependencyResolver);

            ClickOnItem = ReactiveCommand.Create<FootprintInfo?>(_footprintObserverList.ClickOnItem);

            FilterClickCommand = ReactiveCommand.Create(FilterClick);

            this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Subscribe(active => _footprintObserverList.Update());

            this.WhenAnyValue(s => s.Filter).Subscribe(_ => FilterChanged());

            _footprintObserverList.BeginUpdate.Subscribe(_ => MainContent = _updateMainContent);

            _footprintObserverList.EndUpdate.Subscribe(_ => MainContent = _footprintObserverList);

            _footprintObserverList.SelectItem.Subscribe(item =>
            {
                _footrpintLayer.SelectFeature(item.Name);
                _footrpintLayer.DataHasChanged();
                SetMapFocusTo(item.Center);
            });

            _footprintObserverList.UnselectItem.Subscribe(item =>
            {
                _footrpintLayer.UnselectFeature(item.Name);
                _footrpintLayer.DataHasChanged();
            });
        }

        private void FilterChanged()
        {
            if (Filter != null)
            {
                //   Filter.Update += (s, e) => Type = FootprintViewerContentType.Update;
            }
        }

        public ReactiveCommand<FootprintInfo?, Unit> ClickOnItem { get; }

        public ReactiveCommand<Unit, Unit> FilterClickCommand { get; }

        public void SelectFootprintInfo(string name)
        {
            //if (_footrpintLayer != null)
            //{
            //    var isSelect = _footrpintLayer.IsSelect(name);

            //    if (isSelect == true)
            //    {
            //        _footrpintLayer.UnselectFeature(name);

            //        _footprintObserverList.CloseItems();

            //        _footprintObserverList.SelectedFootprintInfo = null;
            //    }
            //    else
            //    {
            //        _footrpintLayer.SelectFeature(name);

            //        var item = _footprintObserverList.FootprintInfos.Where(s => name.Equals(s.Name)).SingleOrDefault();

            //        if (item != null)
            //        {
            //            ScrollCollectionToCenter(item);
            //        }
            //    }
            //}
        }

        //private void ScrollCollectionToCenter(FootprintInfo item)
        //{
        //    ScrollToCenter = true;

        //    _footprintObserverList.SelectedFootprintInfo = item;

        //    ScrollToCenter = false;
        //}

        private void SetMapFocusTo(Coordinate coordinate)
        {
            if (_map != null)
            {
                var p = SphericalMercator.FromLonLat(coordinate.X, coordinate.Y);

                _map.Initialized = false;

                _map.Home = (navigator) =>
                {
                    navigator.CenterOn(new Mapsui.Geometries.Point(p.X, p.Y));
                };

                // HACK: set Map.Initialized to false and add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
                var layer = new Mapsui.Layers.Layer();
                _map.Layers.Add(layer);
                _map.Layers.Remove(layer);
            }
        }

        private void FilterClick()
        {
            Filter?.Click();
        }

        [Reactive]
        public FootprintObserverFilter? Filter { get; set; }

        [Reactive]
        public ReactiveObject? MainContent { get; private set; }

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;
    }
}
