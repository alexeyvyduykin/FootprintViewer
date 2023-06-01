using FootprintViewer;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Helpers;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;
using PlannedScheduleOnMapSample.Layers;
using ReactiveUI.Fody.Helpers;

namespace PlannedScheduleOnMapSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private const int _maxVisibleFootprintStyle = 10000;

    public MainWindowViewModel()
    {
        Map = new Map();
        Map.AddLayer(new Layer(), LayerType.WorldMap);

        var provider = new FootprintProvider();
        var style = CreateFootprintLayerStyle();
        var layer = new DynamicLayer(provider, true) { Name = "FootrpintLayer", Style = style, IsMapInfoLayer = true };
        // var layer = new MemoryLayer() { Name = "FootrpintLayer", Style = style, IsMapInfoLayer = true };
        //var layer = new WritableLayer() { Name = "FootrpintLayer", Style = style, IsMapInfoLayer = true };        
        //var layer = new ObservableMemoryLayer<Footprint>(s => FeatureBuilder.Build(s)) { Name = "FootrpintLayer", Style = style, IsMapInfoLayer = false };

        Map.Layers.Add(layer);

        string path = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "world.mbtiles");
        var resource = new MapResource("world", path);
        Map.SetWorldMapLayer(resource);

        PlannedScheduleTab = new();
        PlannedScheduleTab.ToLayerProvider(provider);
        //  PlannedScheduleTab.ToObservableMemoryLayer(layer);
        // PlannedScheduleTab.ToMemoryLayer(layer);
        // PlannedScheduleTab.ToWritableLayer(layer);
    }

    private static IStyle CreateFootprintLayerStyle()
    {
        return new ThemeStyle(f =>
        {
            if (f is not GeometryFeature gf)
            {
                return null;
            }

            if (gf.Geometry is Point)
            {
                return null;
            }

            //if (gf[SelectField] is true)
            //{
            //    return new VectorStyle()
            //    {
            //        MinVisible = 0,
            //        MaxVisible = _maxVisibleFootprintStyle,
            //        Fill = new Brush(Color.Opacity(mapsuiPalette.Color, 0.55f)),
            //        Outline = new Pen(Color.Black, 4.0),
            //        Line = new Pen(Color.Black, 4.0)
            //    };
            //}

            //if (gf[HoverField] is true)
            //{
            //    return new VectorStyle()
            //    {
            //        MinVisible = 0,
            //        MaxVisible = _maxVisibleFootprintStyle,
            //        Fill = new Brush(Color.Opacity(mapsuiPalette.Color, 0.85f)),
            //        Outline = new Pen(Color.Yellow, 3.0),
            //        Line = new Pen(Color.Yellow, 3.0)
            //    };
            //}

            return new VectorStyle()
            {
                Fill = new Brush(Color.Opacity(Color.Green, 0.25f)),
                Line = new Pen(Color.Green, 1.0),
                Outline = new Pen(Color.Green, 1.0),
                MinVisible = 0,
                MaxVisible = _maxVisibleFootprintStyle,
            };
        });
    }

    public PlannedScheduleTabViewModel PlannedScheduleTab { get; set; }

    public Map Map { get; private set; }

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;
}
