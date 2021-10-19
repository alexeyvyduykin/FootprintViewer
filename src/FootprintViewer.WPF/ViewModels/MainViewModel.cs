using FootprintViewer.Models;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace FootprintViewer.WPF.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public event EventHandler MapInvalidate;
        public event EventHandler CurrentFootprint;

        public MainViewModel()
        {
            Footprints = new ObservableCollection<Footprint>(ResourceManager.GetFootprints());

            Map = SampleBuilder.CreateMap();

            Map.DataChanged += Map_DataChanged;

            this.WhenAnyValue(s => s.SelectedFootprint).Subscribe(footprint =>
            {
                if (footprint != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    Map.Layers.Replace(nameof(LayerType.FootprintLayer), layer);

                    InvalidateMap();

                    CurrentFootprint?.Invoke(this, EventArgs.Empty);
                }
            });

            MouseOverEnterCommand = ReactiveCommand.Create<Footprint>(ShowFootprintBorder);

            MouseOverLeaveCommand = ReactiveCommand.Create(HideFootprintBorder);
        }


        public ReactiveCommand<Footprint, Unit> MouseOverEnterCommand { get; }
        public ReactiveCommand<Unit, Unit> MouseOverLeaveCommand { get; }

        private void ShowFootprintBorder(Footprint footprint)
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

        private void HideFootprintBorder()
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

        private void Map_DataChanged(object sender, Mapsui.Fetcher.DataChangedEventArgs e)
        {
            var list = new List<MapLayer>();

            foreach (var layer in Map.Layers)
            {
                string crs = default;
                string format = default;
                if (layer is TileLayer tileLayer)
                {
                    crs = tileLayer.TileSource.Schema.Srs;
                    format = tileLayer.TileSource.Schema.Format;
                }

                if (string.IsNullOrEmpty(crs) == true)
                {
                    crs = layer.CRS;
                }

                list.Add(new MapLayer()
                {
                    Name = layer.Name,
                    CRS = crs,
                    Format = format,
                });
            }

            MapLayers = new ObservableCollection<MapLayer>(list);
        }

        [Reactive]
        public ObservableCollection<Footprint> Footprints { get; set; }

        [Reactive]
        public Footprint SelectedFootprint { get; set; }

        [Reactive]
        public Map Map { get; set; }

        [Reactive]
        public ObservableCollection<MapLayer> MapLayers { get; set; }

        [Reactive]
        public string AOIDescription { get; set; } = "AOI";

        [Reactive]
        public string RouteDescription { get; set; } = "Route";

        public void InvalidateMap()
        {
            MapInvalidate?.Invoke(this, EventArgs.Empty);
        }
    }

    public class MapLayer
    {
        public string Name { get; set; }

        public string CRS { get; set; }

        public string Format { get; set; }
    }
}
