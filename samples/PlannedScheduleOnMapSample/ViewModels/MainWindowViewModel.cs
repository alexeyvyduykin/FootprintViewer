using BruTile.MbTiles;
using FootprintViewer;
using FootprintViewer.Data;
using FootprintViewer.Helpers;
using FootprintViewer.Layers;
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
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SQLite;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PlannedScheduleOnMapSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private const int _maxVisibleFootprintStyle = 10000;
    private ISelector? _selector;
    private readonly FeatureManager _featureManager;
    private const string SelectField = InteractiveFields.Select;
    private const string HoverField = InteractiveFields.Hover;
    private Subject<IInteractive> _subj = new();

    public static MainWindowViewModel Instance = new();

    public const string WorldKey = "WorldMapLayer";
    public const string SatelliteKey = "SatelliteLayer";
    public const string FootprintKey = "FootprintLayer";
    public const string TrackKey = "TrackLayer";
    public const string PlannedScheduleKey = "PlannedSchedule";
    public const string GroundTargetKey = "GroundTargetLayer";

    private readonly FootprintService _footprintService;
    private readonly DataManager _dataManager = new();

    public MainWindowViewModel()
    {
        _dataManager.RegisterSource(PlannedScheduleKey, new CustomSource());

        var footprintProvider = new FootprintProvider();
        var satelliteProvider = new SatelliteProvider();
        var groundTargetProvider = new GroundTargetProvider();

        Map = new Map();

        Map.Layers.Add(CreateWorldMapLayer());
        Map.Layers.Add(CreateGroundTargetLayer(groundTargetProvider));
        Map.Layers.Add(CreateSatelliteLayer(satelliteProvider));
        Map.Layers.Add(CreateFootprintLayer(footprintProvider));
        Map.Layers.Add(CreateTrackLayer());

        PlannedScheduleTab = new(_dataManager);
        PlannedScheduleTab.ToLayerProvider(footprintProvider);

        SatelliteTab = new(_dataManager);
        SatelliteTab.ToLayerProvider(satelliteProvider);

        GroundTargetTab = new(_dataManager);
        GroundTargetTab.ToLayerProvider(groundTargetProvider);

        MessageBox = new();

        _featureManager = new FeatureManager()
            .WithSelect(f => f[SelectField] = true)
            .WithUnselect(f => f[SelectField] = false)
            .WithEnter(f => f[HoverField] = true)
            .WithLeave(f => f[HoverField] = false);

        _footprintService = new(Map, _dataManager);

        this.WhenAnyValue(s => s.IsFootprintSelector)
            .WhereTrue()
            .Subscribe(_ => SetFootprintSelector());

        this.WhenAnyValue(s => s.IsGroundTargetSelector)
            .WhereTrue()
            .Subscribe(_ => SetGroundTargetSelector());

        IsFootprintSelector = true;
    }

    public IObservable<IInteractive> InteractiveObservable => _subj.AsObservable();

    private static ILayer CreateWorldMapLayer()
    {
        string path = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "world.mbtiles");

        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

        return new TileLayer(mbTilesTileSource)
        {
            Name = WorldKey
        };
    }

    private static ILayer CreateSatelliteLayer(IProvider provider)
    {
        var style = CreateSatelliteLayerStyle();

        var layer = new DynamicLayer(provider, true)
        {
            Name = SatelliteKey,
            Style = style
        };

        return layer;
    }

    private static ILayer CreateGroundTargetLayer(IProvider provider)
    {
        var style = CreateGroundTargetLayerStyle();

        var layer = new DynamicLayer(provider, true)
        {
            Name = GroundTargetKey,
            Style = style
        };

        return layer;
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

    private void SetFootprintSelector()
    {
        _selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .Build();

        var layer1 = Map.Layers.FindLayer(FootprintKey).Single();
        var layer2 = Map.Layers.FindLayer(GroundTargetKey).Single();
        layer1.IsMapInfoLayer = true;
        layer2.IsMapInfoLayer = false;

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

        _subj.OnNext(_selector);
    }

    private void SetGroundTargetSelector()
    {
        _selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .Build();

        var layer1 = Map.Layers.FindLayer(FootprintKey).Single();
        var layer2 = Map.Layers.FindLayer(GroundTargetKey).Single();
        layer1.IsMapInfoLayer = false;
        layer2.IsMapInfoLayer = true;

        _selector.Select.Subscribe(s =>
        {
            SelectFeature(s.Feature, s.Layer);

            MessageBox.ShowGroundTargetFeature(s.Feature);

            // await _footprintService.ShowTrackAsync(s.Feature);
        });

        _selector.Unselect.Subscribe(s =>
        {
            UnselectFeature(s.Layer);
        });

        Interactive = _selector;
        State = States.Selecting;

        _subj.OnNext(_selector);
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
                return new SymbolStyle()
                {
                    Fill = new Brush(Color.Opacity(Color.Black, 0.55f)),
                    Line = new Pen(Color.Black, 1.0),
                    Outline = new Pen(Color.Black, 1.0),
                    SymbolType = SymbolType.Ellipse,
                    SymbolScale = 0.8,
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                };
                // return null;
            }

            if ((string)gf["Name"]! == "Target")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    //  Fill = new Brush(Color.Opacity(Color.Blue, 1.0f)),
                    // Outline = new Pen(Color.Blue, 2.0),
                    Line = new Pen(Color.Opacity(Color.Black, 1.0f), 2.0)
                };
            }

            if ((string)gf["Name"]! == "FootprintTrack")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    //  Fill = new Brush(Color.Opacity(Color.Blue, 1.0f)),
                    // Outline = new Pen(Color.Blue, 2.0),
                    Line = new Pen(Color.Opacity(Color.Blue, 0.65f), 12.0)
                };
            }

            if ((string)gf["Name"]! == "BaseTrack")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    //  Fill = new Brush(Color.Opacity(Color.Black, 0.55f)),
                    //  Outline = new Pen(Color.Black, 1.0),
                    Line = new Pen(Color.Opacity(Color.Black, 0.20f), 12.0)
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
                    // Fill = new Brush(Color.Opacity(Color.Indigo, 0.55f)),
                    Outline = new Pen(Color.Orange, 2.0),
                    Line = new Pen(Color.Orange, 2.0)
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

            if ((string)gf["Name"]! == "AreaPoly")
            {
                return new VectorStyle()
                {
                    Fill = new Brush(Color.Opacity(Color.Orange, 0.25f)),
                    Line = null,
                    Outline = null,
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                };
            }

            return new VectorStyle()
            {
                Fill = null,// new Brush(Color.Opacity(Color.Green, 0.55f)),
                Line = new Pen(Color.Black, 2.0),
                Outline = new Pen(Color.Black, 2.0),
                MinVisible = 0,
                MaxVisible = _maxVisibleFootprintStyle,
            };
        });
    }

    private static IStyle CreateGroundTargetLayerStyle()
    {
        return new ThemeStyle(f =>
        {
            if (f is not GeometryFeature gf)
            {
                return null;
            }

            bool isSelect = gf[SelectField] is true;

            var width = isSelect ? 4.0f : 2.0f;

            if (gf.Geometry is Point)
            {
                return new SymbolStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Black, 0.05f)),
                    Outline = new Pen(Color.Black, width),
                    Line = new Pen(Color.Black, width),
                    SymbolType = SymbolType.Ellipse,
                    SymbolScale = 0.4,
                };
            }

            if ((string)gf["Type"]! == "Route")
            {
                var styleBorder = new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Black, 0.05f)),
                    Outline = new Pen(Color.Opacity(Color.Black, 0.05f), 16.0),
                    Line = new Pen(Color.Opacity(Color.Black, 0.05f), 16.0)
                };

                var style = new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Black, 0.05f)),
                    Outline = new Pen(Color.Black, width),
                    Line = new Pen(Color.Black, width)
                };

                return new StyleCollection
                {
                    Styles = new() { styleBorder, style }
                };
            }

            return new VectorStyle()
            {
                MinVisible = 0,
                MaxVisible = _maxVisibleFootprintStyle,
                Fill = new Brush(Color.Opacity(Color.Black, 0.05f)),
                Outline = new Pen(Color.Black, width),
                Line = new Pen(Color.Black, width)
            };
        });
    }

    private static IStyle CreateSatelliteLayerStyle()
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
                    Line = new Pen(Color.Opacity(Color.Red, 0.65f), 12.0)
                };
            }

            return new VectorStyle()
            {
                Line = new Pen(Color.Opacity(Color.Green, 0.25f), 12.0),
            };
        });
    }

    public PlannedScheduleTabViewModel PlannedScheduleTab { get; set; }

    public SatelliteTabViewModel SatelliteTab { get; set; }

    public GroundTargetTabViewModel GroundTargetTab { get; set; }

    public Map Map { get; private set; }

    [Reactive]
    public MessageBoxViewModel MessageBox { get; set; }

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;

    [Reactive]
    public bool IsFootprintSelector { get; set; }

    [Reactive]
    public bool IsGroundTargetSelector { get; set; }
}
