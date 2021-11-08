using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Models;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Providers.Wfs.Utilities;
using Mapsui.UI;
using Microsoft.VisualBasic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FootprintViewer.ViewModels
{
    public class SceneSearch : SidePanelTab
{
        protected readonly SourceList<Footprint> _sourceFootprints;
        private readonly ReadOnlyObservableCollection<Footprint> _footprints;

        public event EventHandler? CurrentFootprint;
       
        public SceneSearch()
        {
            this.WhenAnyValue(s => s.SelectedFootprint).Subscribe(footprint =>
            {
                if (footprint != null && Map != null && footprint.Path != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    Map.Layers.Replace(nameof(LayerType.FootprintLayer), layer);

                    CurrentFootprint?.Invoke(this, EventArgs.Empty);
                }
            });

            this.WhenAnyValue(s => s.DataSource).Subscribe(_ => DataSourceChanged());

            MouseOverEnterCommand = ReactiveCommand.Create<Footprint>(ShowFootprintBorder);

            MouseOverLeaveCommand = ReactiveCommand.Create(HideFootprintBorder);

            SelectedItemChangedCommand = ReactiveCommand.Create<Footprint>(SelectionChanged);

            Filter = new SceneSearchFilter();
      
            _sourceFootprints = new SourceList<Footprint>();

            var cancellation = _sourceFootprints.Connect()
                .Filter(Filter.Observable)
                .Bind(out _footprints)
                .DisposeMany()
                .Subscribe(_ =>
                {
                    Task.Run(() => longRunningRoutine());
                });        
        }

        private void longRunningRoutine()
        {
            IsUpdating = true;

          //  System.Threading.Thread.Sleep(1000);

            IsUpdating = false;
        }

        private static async Task<IEnumerable<Footprint>> LoadDataAsync(IDataSource dataSource)
        {
            return await Task.Run(() =>
            {
                //Thread.Sleep(5000);
                return dataSource.GetFootprints();
            });
        }

        private async void DataSourceChanged()
        {
            if (DataSource != null)
            {
                var footprints = await LoadDataAsync(DataSource);

                _sourceFootprints.Clear();
                _sourceFootprints.AddRange(footprints);

                var sortNames = new List<Footprint>(footprints).Select(s => s.SatelliteName).Distinct().ToList();
                sortNames.Sort();

                Filter.AddSensors(sortNames);
            }
        }



        public ReactiveCommand<Footprint, Unit> MouseOverEnterCommand { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeaveCommand { get; }

        public ReactiveCommand<Footprint, Unit> SelectedItemChangedCommand { get; }

        private void ShowFootprintBorder(Footprint footprint)
        {
            if (Map != null)
            {
                var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintBorderLayer));

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
                var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintBorderLayer));

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

        private void SelectionChanged(Footprint footprint)
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
        public IDataSource? DataSource { get; set; }

        [Reactive]
        public Map? Map { get; set; }


        public ReadOnlyObservableCollection<Footprint> Footprints => _footprints;

      //  [Reactive]
     //   public ObservableCollection<Footprint> Footprints { get; private set; } = new ObservableCollection<Footprint>();

        [Reactive]
        public Footprint? SelectedFootprint { get; set; }

        [Reactive]
        public SceneSearchFilter Filter { get; set; }

        [Reactive]
        public bool IsUpdating { get; set; } = false;
    }

    public class SceneSearchDesigner : SceneSearch
    {
        public SceneSearchDesigner() : base()
        {
            var list = new List<Footprint>();

            Random random = new Random();

            var names = new[] { "02-65-lr_2000-3857-lite", "36-65-ur_2000-3857-lite", "38-50-ll_3857-lite", "38-50-lr_3857-lite", "38-50-ul_3857-lite", "38-50-ur_3857-lite", "41-55-ul_2000-3857-lite", "44-70-ur_2000-3857-lite" };
            var satellites = new[] { "Satellite1", "Satellite2", "Satellite3" };

            foreach (var item in names)
            {
                var name = item.Replace("lite", "").Replace("2000", "").Replace("3857", "").Replace("_", "").Replace("-", "");
                var date = DateTime.UtcNow;

                list.Add(new Footprint() 
                {
                    Date = date.Date.ToShortDateString(),
                    SatelliteName = satellites[random.Next(0, satellites.Length - 1)],
                    SunElevation = random.Next(0, 90),
                    CloudCoverFull = random.Next(0, 100),
                    TileNumber = name.ToUpper(),
                });
            }

            AddFootprints(list);
        }

        public void AddFootprints(IEnumerable<Footprint> footprints)
        {            
            _sourceFootprints.Clear();
            _sourceFootprints.AddRange(footprints);

            var sortNames = Footprints.Select(s => s.SatelliteName).Distinct().ToList();
            sortNames.Sort();

            Filter.AddSensors(sortNames); 
        }
    }
}
