using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Projection;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public enum FootprintViewerContentType
    {
        Show,
        Update
    }

    public class FootprintObserver : SidePanelTab
    {
        private readonly FootprintLayer? _footrpintLayer;
        private readonly Map? _map;
        private readonly FootprintObserverList _footprintObserverList;   
        private readonly PreviewMainContent _updateMainContent;

        public FootprintObserver(IReadonlyDependencyResolver dependencyResolver)
        {
            var map = dependencyResolver.GetService<Map>();

            _map = map;

            Title = "Просмотр рабочей программы";

            Name = "FootprintViewer";
                
            _footrpintLayer = map?.GetLayer<FootprintLayer>(LayerType.Footprint);
                                                                     
            Filter = new FootprintObserverFilter(dependencyResolver);
            
            _updateMainContent = new PreviewMainContent("Загрузка...");

            _footprintObserverList = new FootprintObserverList(dependencyResolver);

            //PreviewMouseLeftButtonDownCommand = ReactiveCommand.Create(PreviewMouseLeftButtonDown);

            ClickOnItem = ReactiveCommand.Create<FootprintInfo?>(ClickOnItemImpl);

            FilterClickCommand = ReactiveCommand.Create(FilterClick);

            this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Subscribe(active => _footprintObserverList.Update());

            _footprintObserverList.SelectedFootprintInfoObserver.WhereNotNull().Subscribe(footprint =>
            {
                if (ScrollToCenter == false)
                {
                    if (string.IsNullOrEmpty(footprint.Name) == false && _footrpintLayer != null)
                    {
                        _footrpintLayer.SelectFeature(footprint.Name);
                    }

                    SetMapFocusTo(footprint.Center);
                }
            });

            this.WhenAnyValue(s => s.PreviewMouseLeftButtonCommandChecker).Subscribe(_ =>
            {
                if (_footprintObserverList.SelectedFootprintInfo != null && string.IsNullOrEmpty(_footprintObserverList.SelectedFootprintInfo.Name) == false)
                {
                    if (_footrpintLayer != null)
                    {
                        if (_footprintObserverList.SelectedFootprintInfo.IsShowInfo == true)
                        {
                            _footrpintLayer.SelectFeature(_footprintObserverList.SelectedFootprintInfo.Name);
                        }
                        else
                        {
                            _footrpintLayer.UnselectFeature(_footprintObserverList.SelectedFootprintInfo.Name);
                        }

                        _footrpintLayer.DataHasChanged();
                    }
                }
            });

            this.WhenAnyValue(s => s.Filter).Subscribe(_ => FilterChanged());

            _footprintObserverList.BeginUpdate.Subscribe(_ => MainContent = _updateMainContent);

            _footprintObserverList.EndUpdate.Subscribe(_ => MainContent = _footprintObserverList);
        }

        private void FilterChanged()
        {
            if (Filter != null)
            {
                //   Filter.Update += (s, e) => Type = FootprintViewerContentType.Update;
            }
        }

        //public ReactiveCommand<Unit, Unit> PreviewMouseLeftButtonDownCommand { get; }

        public ReactiveCommand<FootprintInfo?, Unit> ClickOnItem { get; }

        public ReactiveCommand<Unit, Unit> FilterClickCommand { get; }

        public void SelectFootprintInfo(string name)
        {
            if (_footrpintLayer != null)
            {
                var isSelect = _footrpintLayer.IsSelect(name);

                if (isSelect == true)
                {
                    _footrpintLayer.UnselectFeature(name);

                    _footprintObserverList.CloseItems();

                    _footprintObserverList.SelectedFootprintInfo = null;
                }
                else
                {
                    _footrpintLayer.SelectFeature(name);

                    var item = _footprintObserverList.FootprintInfos.Where(s => name.Equals(s.Name)).SingleOrDefault();

                    if (item != null)
                    {
                        ScrollCollectionToCenter(item);
                    }
                }
            }
        }

        private void ScrollCollectionToCenter(FootprintInfo item)
        {
            ScrollToCenter = true;

            _footprintObserverList.SelectedFootprintInfo = item;

            ScrollToCenter = false;
        }

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

        //private void PreviewMouseLeftButtonDown()
        //{
        //    if (SelectedFootprintInfo != null)
        //    {
        //        if (SelectedFootprintInfo.IsShowInfo == true)
        //        {
        //            SelectedFootprintInfo.IsShowInfo = false;
        //        }
        //        else
        //        {
        //            SelectedFootprintInfo.IsShowInfo = true;
        //        }

        //        PreviewMouseLeftButtonCommandChecker = !PreviewMouseLeftButtonCommandChecker;
        //    }
        //}

        private void ClickOnItemImpl(FootprintInfo? item)
        {
            if (_footprintObserverList.SelectedFootprintInfo != null)
            {
                if (_footprintObserverList.SelectedFootprintInfo.IsShowInfo == true)
                {
                    _footprintObserverList.SelectedFootprintInfo.IsShowInfo = false;
                }
                else
                {
                    _footprintObserverList.SelectedFootprintInfo.IsShowInfo = true;
                }

                PreviewMouseLeftButtonCommandChecker = !PreviewMouseLeftButtonCommandChecker;
            }

            _footprintObserverList.SelectedFootprintInfo = item;
        }

        private void FilterClick()
        {
            Filter?.Click();
        }

        private static async Task<IList<Footprint>> LoadDataAsync(FootprintLayer layer)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(500);
                return layer.GetFootprintsAsync();
            });
        }

        [Reactive]
        public FootprintObserverFilter? Filter { get; set; }

        [Reactive]
        public ReactiveObject? MainContent { get; private set; }

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;

        [Reactive]
        private bool PreviewMouseLeftButtonCommandChecker { get; set; } = false;
    }
}
