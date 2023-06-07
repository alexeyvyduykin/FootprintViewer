using BruTile.MbTiles;
using FootprintViewer.Data;
using FootprintViewer.Helpers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.Tiling.Layers;
using NetTopologySuite.Geometries;
using PlannedScheduleOnMapSample.Layers;
using PlannedScheduleOnMapSample.Models;
using ReactiveUI.Fody.Helpers;
using SQLite;
using System;

namespace PlannedScheduleOnMapSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private const int _maxVisibleFootprintStyle = 10000;
    private ISelector? _selector;
    private readonly FeatureManager _featureManager;
    private const string SelectField = InteractiveFields.Select;
    private const string HoverField = InteractiveFields.Hover;

    public static MainWindowViewModel Instance = new();

    public const string WorldKey = "WorldMapLayer";
    public const string FootprintKey = "FootprintLayer";
    public const string TrackKey = "TrackLayer";
    public const string PlannedScheduleKey = "PlannedSchedule";

    private readonly FootprintService _footprintService;
    private readonly DataManager _dataManager = new();

    public MainWindowViewModel()
    {
        _dataManager.RegisterSource(PlannedScheduleKey, new CustomSource());

        var provider = new FootprintProvider();

        Map = new Map();

        Map.Layers.Add(CreateWorldMapLayer());
        Map.Layers.Add(CreateFootprintLayer(provider));
        Map.Layers.Add(CreateTrackLayer());

        PlannedScheduleTab = new(_dataManager);
        PlannedScheduleTab.ToLayerProvider(provider);

        MessageBox = new();

        _featureManager = new FeatureManager()
            .WithSelect(f => f[SelectField] = true)
            .WithUnselect(f => f[SelectField] = false)
            .WithEnter(f => f[HoverField] = true)
            .WithLeave(f => f[HoverField] = false);

        _footprintService = new(Map, _dataManager);

        SelectCommand();
    }

    private static ILayer CreateWorldMapLayer()
    {
        string path = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "world.mbtiles");

        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

        return new TileLayer(mbTilesTileSource)
        {
            Name = WorldKey
        };
    }

    private static ILayer CreateFootprintLayer(IProvider provider)
    {
        var style = CreateFootprintLayerStyle();

        var layer = new Layer()
        {
            Name = FootprintKey,
            DataSource = provider,
            Style = style,
            IsMapInfoLayer = true
        };

        return layer;
    }

    private static ILayer CreateTrackLayer()
    {
        var style = CreateTrackLayerStyle();

        var layer = new QueueLayer()
        {
            Name = TrackKey,
            Style = style,
            IsMapInfoLayer = false
        };

        return layer;
    }

    private void SelectCommand()
    {
        _selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .Build();

        _selector.Select.Subscribe(async s =>
        {
            SelectFeature(s.Feature, s.Layer);

            MessageBox.ShowFootprintFeature(s.Feature);

            await _footprintService.ShowTrackAsync(s.Feature);
        });

        _selector.Unselect.Subscribe(s =>
        {
            UnselectFeature(s.Layer);
        });

        //_selector.HoverBegin.Subscribe(s =>
        //{
        //    EnterFeature(s.Feature, s.Layer);
        //});

        //_selector.HoverEnd.Subscribe(s =>
        //{
        //    LeaveFeature(s.Layer);
        //});

        Interactive = _selector;
        State = States.Selecting;
    }

    private void EnterFeature(IFeature feature, ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Enter(feature);
    }

    private void LeaveFeature(ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Leave();
    }

    private void SelectFeature(IFeature feature, ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Select(feature);
    }

    private void UnselectFeature(ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Unselect();
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

            if (gf[SelectField] is true)
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Green, 0.55f)),
                    Outline = new Pen(Color.Black, 4.0),
                    Line = new Pen(Color.Black, 4.0)
                };
            }

            if (gf[HoverField] is true)
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Green, 0.85f)),
                    Outline = new Pen(Color.Yellow, 3.0),
                    Line = new Pen(Color.Yellow, 3.0)
                };
            }

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

    private static IStyle CreateTrackLayerStyle()
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

            if ((string)gf["Name"]! == "FootprintTrack")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Blue, 1.0f)),
                    Outline = new Pen(Color.Blue, 2.0),
                    Line = new Pen(Color.Blue, 2.0)
                };
            }

            if ((string)gf["Name"]! == "BaseTrack")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Yellow, 0.55f)),
                    Outline = new Pen(Color.Yellow, 1.0),
                    Line = new Pen(Color.Yellow, 1.0)
                };
            }

            if ((string)gf["Name"]! == "FootprintSwath")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Orange, 1.0f)),
                    Outline = new Pen(Color.Orange, 2.0),
                    Line = new Pen(Color.Orange, 2.0)
                };
            }

            if ((string)gf["Name"]! == "BaseSwath")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Indigo, 0.55f)),
                    Outline = new Pen(Color.Indigo, 1.0),
                    Line = new Pen(Color.Indigo, 1.0)
                };
            }

            if ((string)gf["Name"]! == "Arrow")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Red, 1.0f)),
                    Outline = new Pen(Color.Red, 1.0),
                    Line = new Pen(Color.Red, 1.0)
                };
            }

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
    public MessageBoxViewModel MessageBox { get; set; }

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;
}
