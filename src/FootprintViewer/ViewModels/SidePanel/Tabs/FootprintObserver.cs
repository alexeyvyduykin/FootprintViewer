using FootprintViewer.Data;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Projections;
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
        private readonly Map _map;
        private readonly FootprintObserverList _footprintObserverList;
        private readonly FootprintObserverFilter _filter;

        public FootprintObserver(IReadonlyDependencyResolver dependencyResolver)
        {
            var map = dependencyResolver.GetExistingService<Map>();
            var footprintProvider = dependencyResolver.GetExistingService<FootprintProvider>();

            _filter = new FootprintObserverFilter(dependencyResolver);

            _footprintObserverList = new FootprintObserverList(footprintProvider);

            _map = map;

            Title = "Просмотр рабочей программы";

            ClickOnItem = ReactiveCommand.Create<FootprintInfo?, FootprintInfo?>(s => { _footprintObserverList.ClickOnItem(s); return s; });

            FilterClick = ReactiveCommand.Create(FilterClickImpl);

            this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Select(_ => _filter).InvokeCommand(_footprintObserverList.Loading);

            this.WhenAnyValue(s => s.IsActive).Where(active => active == false).Subscribe(_ => IsFilterOpen = false);

            _footprintObserverList.SelectItem.Subscribe(item =>
            {
                SetMapFocusTo(item.Center);
            });

            _filter.Update.Select(filter => filter).InvokeCommand(_footprintObserverList.Loading);

            this.WhenAnyValue(s => s.IsExpanded).Where(c => c == false).Subscribe(_ => IsFilterOpen = false);

            MainContent = _footprintObserverList;
        }

        //private void FootprintsChanged(FootprintObserverFilter? filter = null)
        //{
        //    if (_footrpintLayer != null)
        //    {
        //        if (filter == null)
        //        {
        //            _footprintObserverList.InvalidateData(() => _footrpintLayer.GetFootprints());
        //        }
        //        else
        //        {
        //            _footprintObserverList.InvalidateData(() => _footrpintLayer.GetFootprints().Where(s => filter.Filtering(new FootprintInfo(s))));
        //        }
        //    }
        //}

        public ReactiveCommand<FootprintInfo?, FootprintInfo?> ClickOnItem { get; }

        public ReactiveCommand<Unit, Unit> FilterClick { get; }

        public void SelectFootprintInfo(string name)
        {
            _footprintObserverList.SelectFootprintInfo(name);
        }

        public FootprintInfo? GetFootprintInfo(string name)
        {
            return _footprintObserverList.GetFootprintInfo(name);
        }

        private void FilterClickImpl()
        {
            IsFilterOpen = !IsFilterOpen;
        }

        private void SetMapFocusTo(Coordinate coordinate)
        {
            if (_map != null)
            {
                var p = SphericalMercator.FromLonLat(coordinate.X, coordinate.Y).ToMPoint();

                _map.Initialized = false;

                _map.Home = (navigator) =>
                {
                    navigator.CenterOn(new MPoint(p.X, p.Y));
                };

                // HACK: set Map.Initialized to false and add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
                var layer = new Mapsui.Layers.Layer();
                _map.Layers.Add(layer);
                _map.Layers.Remove(layer);
            }
        }

        public FootprintObserverFilter Filter => _filter;

        [Reactive]
        public bool IsFilterOpen { get; private set; }

        [Reactive]
        public ReactiveObject? MainContent { get; private set; }

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;
    }
}
