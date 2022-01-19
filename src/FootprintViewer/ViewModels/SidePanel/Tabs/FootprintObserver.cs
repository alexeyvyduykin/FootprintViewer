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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
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
        private readonly FootprintObserverList? _footprintObserverList;
        private readonly PreviewMainContent _previewMainContent;

        public FootprintObserver(IReadonlyDependencyResolver dependencyResolver)
        {
            var map = dependencyResolver.GetService<Map>();

            _map = map;

            Title = "Просмотр рабочей программы";

            Name = "FootprintViewer";
                
            _footrpintLayer = map?.GetLayer<FootprintLayer>(LayerType.Footprint);
                    
            _footprintObserverList = new FootprintObserverList(dependencyResolver);
                                            
            Filter = new FootprintObserverFilter(dependencyResolver);
            
            FootprintInfos = new ObservableCollection<FootprintInfo>();

            _previewMainContent = new PreviewMainContent("Загрузка...");


            //   _footprintObserverList.LoadFootprints.Subscribe(_ => MainContent = _footprintObserverList);

            PreviewMouseLeftButtonDownCommand = ReactiveCommand.Create(PreviewMouseLeftButtonDown);

            ClickOnItem = ReactiveCommand.Create<FootprintInfo?>(ClickOnItemImpl);

            FilterClickCommand = ReactiveCommand.Create(FilterClick);

            this.WhenAnyValue(s => s.IsActive).Subscribe(active =>
            {
                if (active == true)
                {
                    UpdateList();           
                }
            });

            this.WhenAnyValue(s => s.SelectedFootprintInfo).Subscribe(footprint =>
            {
                if (footprint != null)
                {
                    FootprintInfos.ToList().ForEach(s => s.IsShowInfo = false);

                    footprint.IsShowInfo = true;

                    if (ScrollToCenter == false)
                    {
                        if (string.IsNullOrEmpty(footprint.Name) == false && _footrpintLayer != null)
                        {
                            _footrpintLayer.SelectFeature(footprint.Name);
                        }

                        SetMapFocusTo(footprint.Center);
                    }
                }
            });

            this.WhenAnyValue(s => s.PreviewMouseLeftButtonCommandChecker).Subscribe(_ =>
            {
                if (SelectedFootprintInfo != null && string.IsNullOrEmpty(SelectedFootprintInfo.Name) == false)
                {
                    if (_footrpintLayer != null)
                    {
                        if (SelectedFootprintInfo.IsShowInfo == true)
                        {
                            _footrpintLayer.SelectFeature(SelectedFootprintInfo.Name);
                        }
                        else
                        {
                            _footrpintLayer.UnselectFeature(SelectedFootprintInfo.Name);
                        }

                        _footrpintLayer.DataHasChanged();
                    }
                }
            });

            this.WhenAnyValue(s => s.Filter).Subscribe(_ => FilterChanged());

            MainContent = _previewMainContent;
        }

        private void UpdateList()
        {
            //   MainContent = _previewMainContent;

            if (_footprintObserverList != null)
            {
                _footprintObserverList.LoadFootprints.Execute().Subscribe();

                MainContent = _footprintObserverList;
            }
        }

        private void FilterChanged()
        {
            if (Filter != null)
            {
                //   Filter.Update += (s, e) => Type = FootprintViewerContentType.Update;
            }
        }

        public ReactiveCommand<Unit, Unit> PreviewMouseLeftButtonDownCommand { get; }

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

                    FootprintInfos.ToList().ForEach(s => s.IsShowInfo = false);

                    SelectedFootprintInfo = null;
                }
                else
                {
                    _footrpintLayer.SelectFeature(name);

                    var item = FootprintInfos.Where(s => name.Equals(s.Name)).SingleOrDefault();

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

            SelectedFootprintInfo = item;

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

        private void PreviewMouseLeftButtonDown()
        {
            if (SelectedFootprintInfo != null)
            {
                if (SelectedFootprintInfo.IsShowInfo == true)
                {
                    SelectedFootprintInfo.IsShowInfo = false;
                }
                else
                {
                    SelectedFootprintInfo.IsShowInfo = true;
                }

                PreviewMouseLeftButtonCommandChecker = !PreviewMouseLeftButtonCommandChecker;
            }
        }

        private void ClickOnItemImpl(FootprintInfo? item)
        {
            if (SelectedFootprintInfo != null)
            {
                if (SelectedFootprintInfo.IsShowInfo == true)
                {
                    SelectedFootprintInfo.IsShowInfo = false;
                }
                else
                {
                    SelectedFootprintInfo.IsShowInfo = true;
                }

                PreviewMouseLeftButtonCommandChecker = !PreviewMouseLeftButtonCommandChecker;
            }

            SelectedFootprintInfo = item;
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

        private async void FootprintsChanged()
        {
            if (_footrpintLayer != null)
            {
                var footprints = await LoadDataAsync(_footrpintLayer);

                if (footprints != null)
                {
                    if (Filter == null)
                    {
                        FootprintInfos = new ObservableCollection<FootprintInfo>(footprints.Select(s => new FootprintInfo(s)));
                    }

                    if (Filter != null)
                    {
                        var list = new List<FootprintInfo>();

                        foreach (var item in footprints)
                        {
                            if (Filter.Filtering(item) == true)
                            {
                                list.Add(new FootprintInfo(item));
                            }
                        }

                        FootprintInfos = new ObservableCollection<FootprintInfo>(list);
                    }
                }

                //      Type = FootprintViewerContentType.Show;
            }
        }

        [Reactive]
        public FootprintObserverFilter? Filter { get; set; }

        public FootprintObserverList? FootprintObserverList => _footprintObserverList;

        //  [Reactive]
        //   public FootprintViewerContentType Type { get; set; }

        [Reactive]
        public ReactiveObject MainContent { get; private set; }

        [Reactive]
        public FootprintInfo? SelectedFootprintInfo { get; set; }

        [Reactive]
        public ObservableCollection<FootprintInfo> FootprintInfos { get; set; }

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;

        [Reactive]
        private bool PreviewMouseLeftButtonCommandChecker { get; set; } = false;
    }
}
