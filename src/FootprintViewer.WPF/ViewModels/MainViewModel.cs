using FootprintViewer.Models;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace FootprintViewer.WPF.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public event EventHandler CurrentFootprint;

        public MainViewModel()
        {
            Footprints = new ObservableCollection<Footprint>(ResourceManager.GetFootprints());

            ActualController = new EditController();

            Map = SampleBuilder.CreateMap();

            var editLayer = (EditLayer)Map.Layers.First(l => l.Name == nameof(LayerType.EditLayer));

            InteractiveFeatureObserver = new InteractiveFeatureObserver(editLayer);

            InteractiveFeatureObserver.CreatingCompleted += FeatureEndCreating;

            InteractiveFeatureObserver.HoverCreating += FeatureHoverCreating;

            InteractiveFeatureObserver.StepCreating += FeatureStepCreating;

            Map.DataChanged += Map_DataChanged;

            this.WhenAnyValue(s => s.SelectedFootprint).Subscribe(footprint =>
            {
                if (footprint != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    Map.Layers.Replace(nameof(LayerType.FootprintLayer), layer);

                    CurrentFootprint?.Invoke(this, EventArgs.Empty);
                }
            });

            MouseOverEnterCommand = ReactiveCommand.Create<Footprint>(ShowFootprintBorder);

            MouseOverLeaveCommand = ReactiveCommand.Create(HideFootprintBorder);

            ToolRectangleAOICommand = new RelayCommand(_ => ActualController = new DrawRectangleController());

            ToolPolygonAOICommand = ReactiveCommand.Create(() => { ActualController = new DrawPolygonController(); });

            ToolCircleAOICommand = new RelayCommand(_ => ActualController = new DrawCircleController());

            ToolRouteDistanceCommand = new RelayCommand(_ =>
            {
                var layer = (EditLayer)Map.Layers.FirstOrDefault(l => l.Name == nameof(LayerType.EditLayer));
                if (layer != null)
                {
                    layer.ClearRoute();
                }

                ActualController = new DrawRouteController();
            });

            ToolEditCommand = new RelayCommand(_ => ActualController = new EditController());
        }

        public ReactiveCommand<Footprint, Unit> MouseOverEnterCommand { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeaveCommand { get; }

        // tools
        public ICommand ToolRectangleAOICommand { get; }

        public ReactiveCommand<Unit, Unit> ToolPolygonAOICommand { get; }

        public ICommand ToolCircleAOICommand { get; }

        public ICommand ToolRouteDistanceCommand { get; }

        public ICommand ToolEditCommand { get; }

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

        private void FeatureEndCreating(object? sender, FeatureEventArgs e)
        {
            var feature = e.Feature;
            var bb = feature.Geometry.BoundingBox;
            var coord = ProjectHelper.ToString(bb.Centroid);
            var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
            var area = SphericalUtil.ComputeSignedArea(vertices);
            area = Math.Abs(area);

            AOIDescription = $"{area:N2} km² | {coord}";
            //RouteDescription;
        }

        private void FeatureHoverCreating(object? sender, FeatureEventArgs e)
        {
            var feature = e.Feature;
            var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
            var area = SphericalUtil.ComputeSignedArea(vertices);
            area = Math.Abs(area);

            AOIHoverDescription = $"{area:N2} km²";
            //RouteHoverDescription;
        }

        private void FeatureStepCreating(object? sender, FeatureEventArgs e)
        {
            var feature = e.Feature;

            if (feature["Name"].Equals(FeatureType.Route.ToString()) == true)
            {
                var geometry = (LineString)feature.Geometry;
                var vertices = geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
                var distance = SphericalUtil.ComputeDistance(vertices);

                RouteDescription = $"{distance:N2} km";
            }
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

        [Reactive]
        public string AOIHoverDescription { get; set; } = "Hover AOI";

        [Reactive]
        public string RouteHoverDescription { get; set; } = "Hover Route";

        [Reactive]
        public IController ActualController { get; set; }

        [Reactive]
        public IInteractiveFeatureObserver InteractiveFeatureObserver { get; set; }
    }

    public class MapLayer
    {
        public string Name { get; set; }

        public string CRS { get; set; }

        public string Format { get; set; }
    }
}
