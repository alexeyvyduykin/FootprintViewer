using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Models;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class SceneSearch : SidePanelTab
    {
        private readonly IList<FootprintImage> _sourceFootprints = new List<FootprintImage>();
        // protected readonly SourceList<Footprint> _sourceFootprints;
        // private readonly ReadOnlyObservableCollection<Footprint> _footprints;

        public event EventHandler? CurrentFootprint;

        public SceneSearch()
        {
            var map = Locator.Current.GetService<Map>();
            var userDataSource = Locator.Current.GetService<UserDataSource>();

            Title = "Поиск сцены";
            Name = "Scene";

            this.WhenAnyValue(s => s.SelectedFootprint).Subscribe(footprint =>
            {
                if (footprint != null && Map != null && footprint.Path != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    Map.Layers.Replace(nameof(LayerType.FootprintImage), layer);

                    CurrentFootprint?.Invoke(this, EventArgs.Empty);
                }
            });

            this.WhenAnyValue(s => s.UserDataSource).Subscribe(_ => UserDataSourceChanged());

            MouseOverEnterCommand = ReactiveCommand.Create<FootprintImage>(ShowFootprintBorder);

            MouseOverLeaveCommand = ReactiveCommand.Create(HideFootprintBorder);

            FilterClickCommand = ReactiveCommand.Create(FilterClick);

            SelectedItemChangedCommand = ReactiveCommand.Create<FootprintImage>(SelectionChanged);

            Filter = new SceneSearchFilter();

            Filter.FromDate = DateTime.Today.AddDays(-1);
            Filter.ToDate = DateTime.Today.AddDays(1);

            Filter.Update += Filter_Update;

            //_sourceFootprints = new SourceList<Footprint>();

            //var cancellation = _sourceFootprints.Connect()
            //    .Filter(Filter.Observable)
            //    .Bind(out _footprints)
            //    .DisposeMany()
            //    .Subscribe(_ =>
            //    {
            //        Task.Run(() => longRunningRoutine());
            //    });        

            UserDataSource = userDataSource;
            Map = map;
        }

        private void Filter_Update(object? sender, EventArgs e)
        {
            if (sender is SceneSearchFilter filter)
            {
                IsUpdating = true;

                Footprints.Clear();

                foreach (var item in _sourceFootprints)
                {
                    if (filter.Filtering(item) == true)
                    {
                        Footprints.Add(item);
                    }
                }

                IsUpdating = false;
            }
        }

        public void SetAOI(IGeometry aoi)
        {
            Filter.AOI = aoi;

            Filter.ForceUpdate();
        }

        public void ResetAOI()
        {
            Filter.AOI = null;

            Filter.ForceUpdate();
        }

        private void longRunningRoutine()
        {
            IsUpdating = true;

            //  System.Threading.Thread.Sleep(1000);

            IsUpdating = false;
        }

        private static async Task<IEnumerable<FootprintImage>> LoadDataAsync(IUserDataSource dataSource)
        {
            return await Task.Run(() =>
            {
                //Thread.Sleep(5000);
                return dataSource.GetFootprints();
            });
        }

        private async void UserDataSourceChanged()
        {
            if (UserDataSource != null)
            {
                var footprints = await LoadDataAsync(UserDataSource);

                // _sourceFootprints.Clear();
                // _sourceFootprints.AddRange(footprints);

                _sourceFootprints.AddRange(footprints);

               // Footprints.Clear();
               // Footprints.AddRange(footprints);

                Footprints = new ObservableCollection<FootprintImage>(footprints);

                var sortNames = new List<FootprintImage>(footprints).Select(s => s.SatelliteName).Distinct().ToList();
                sortNames.Sort();

                Filter.AddSensors(sortNames);
            }
        }

        public ReactiveCommand<Unit, Unit> FilterClickCommand { get; }

        public ReactiveCommand<FootprintImage, Unit> MouseOverEnterCommand { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeaveCommand { get; }

        public ReactiveCommand<FootprintImage, Unit> SelectedItemChangedCommand { get; }

        private void ShowFootprintBorder(FootprintImage footprint)
        {
            if (Map != null)
            {
                var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintImageBorder));

                if (layers != null)
                {
                    var layer = layers.SingleOrDefault();

                    if (layer != null && layer is WritableLayer writableLayer)
                    {
                        writableLayer.Clear();
                        writableLayer.Add(new Feature() { Geometry = footprint.Geometry });
                        writableLayer.DataHasChanged();
                    }
                }
            }
        }

        private void HideFootprintBorder()
        {
            if (Map != null)
            {
                var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintImageBorder));

                if (layers != null)
                {
                    var layer = layers.SingleOrDefault();

                    if (layer != null && layer is WritableLayer writableLayer)
                    {
                        writableLayer.Clear();
                        writableLayer.DataHasChanged();
                    }
                }
            }
        }

        private void FilterClick()
        {
            Filter.Click();
        }

        private void SelectionChanged(FootprintImage footprint)
        {
            if (Map != null && footprint != null && footprint.Geometry != null)
            {
                var point = footprint.Geometry.BoundingBox.Centroid;

                Map.Initialized = false;

                Map.Home = (navigator) =>
                {
                    navigator.CenterOn(point);
                };

                // HACK: set Map.Initialized to false and add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
                var layer = new Mapsui.Layers.Layer();
                Map.Layers.Add(layer);
                Map.Layers.Remove(layer);
            }
        }

        [Reactive]
        public IUserDataSource? UserDataSource { get; set; }

        [Reactive]
        public Map? Map { get; set; }

        // public ReadOnlyObservableCollection<Footprint> Footprints => _footprints;

        [Reactive]
        public ObservableCollection<FootprintImage> Footprints { get; private set; } = new ObservableCollection<FootprintImage>();

        [Reactive]
        public FootprintImage? SelectedFootprint { get; set; }

        [Reactive]
        public SceneSearchFilter Filter { get; set; }

        [Reactive]
        public bool IsUpdating { get; set; } = false;
    }
}
