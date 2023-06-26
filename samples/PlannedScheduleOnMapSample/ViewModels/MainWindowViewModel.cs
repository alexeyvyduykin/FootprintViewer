using BruTile.MbTiles;
using FootprintViewer;
using FootprintViewer.Data;
using FootprintViewer.Data.Models;
using FootprintViewer.Helpers;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Tiling.Layers;
using PlannedScheduleOnMapSample.Layers;
using PlannedScheduleOnMapSample.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SQLite;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

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
    public const string FootprintPreviewKey = "FootprintPreviewLayer";
    public const string TrackKey = "TrackLayer";
    public const string PlannedScheduleKey = "PlannedSchedule";
    public const string GroundTargetKey = "GroundTargetLayer";

    private readonly FootprintService _footprintService;
    private readonly DataManager _dataManager = new();
    private readonly FootprintProvider _footprintProvider;
    private readonly FootprintPreviewProvider _footprintPreviewProvider;
    private readonly ILayer _footprintLayer;
    private readonly ILayer _footprintPreviewLayer;

    public MainWindowViewModel()
    {
        _dataManager.RegisterSource(PlannedScheduleKey, new CustomSource());

        _footprintProvider = new FootprintProvider();
        _footprintPreviewProvider = new FootprintPreviewProvider(_dataManager);
        var satelliteProvider = new SatelliteProvider();
        var groundTargetProvider = new GroundTargetProvider();

        Map = new Map();

        Map.Layers.Add(CreateWorldMapLayer());
        Map.Layers.Add(CreateGroundTargetLayer(groundTargetProvider));
        Map.Layers.Add(CreateSatelliteLayer(satelliteProvider));
        _footprintLayer = CreateFootprintLayer(_footprintProvider);
        _footprintPreviewLayer = CreateFootprintPreviewLayer(_footprintPreviewProvider);
        Map.Layers.Add(_footprintLayer);
        Map.Layers.Add(_footprintPreviewLayer);
        Map.Layers.Add(CreateTrackLayer());

        PlannedScheduleTab = new(_dataManager);
        PlannedScheduleTab.ToLayerProvider(_footprintProvider);
        PlannedScheduleTab.ToLayerProvider(_footprintPreviewProvider);

        SatelliteTab = new(_dataManager);
        SatelliteTab.ToLayerProvider(satelliteProvider);

        GroundTargetTab = new(_dataManager);
        GroundTargetTab.ToLayerProvider(groundTargetProvider);

        MessageBox = new();

        _featureManager = new FeatureManager()
            .WithSelectStyle(StyleBuilder.CreateSelectFootprintStyle())
            .WithHoverStyle(StyleBuilder.CreateHoverFootprintStyle());

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

    public FeatureManager FeatureManager => _featureManager;

    public void SelectFootprint(string name)
    {
        var feature = _footprintProvider.Find(name, "Name")!;

        FeatureManager.Select(feature);

        _footprintLayer.DataHasChanged();
    }

    public void FootprintDimming(bool active)
    {
        _footprintLayer.Style = StyleBuilder.CreateFootprintStyle(active);

        _footprintLayer.DataHasChanged();
    }

    public async Task SelectFootprint(string name, bool isPreview)
    {
        var feature = _footprintProvider.Find(name, "Name")!;

        FeatureManager.Select(feature);

        if (isPreview == true)
        {
            await _footprintService.ShowTrackAsync(feature);
        }

        _footprintLayer.DataHasChanged();
    }

    public void SelectFootprint(string name, ObservationTaskResult taskResult, bool isPreview, bool isFullTrack, bool isSwath, bool isGt)
    {
        var feature = _footprintProvider.Find(name, "Name")!;

        FeatureManager.Select(feature);

        _footprintPreviewProvider.Update(taskResult, isPreview, isFullTrack, isSwath, isGt);

        _footprintLayer.DataHasChanged();
        _footprintPreviewLayer.DataHasChanged();
    }

    public void EnterFootprint(string name)
    {
        var feature = _footprintProvider.Find(name, "Name")!;

        FeatureManager.Enter(feature);

        _footprintLayer.DataHasChanged();
    }

    public void LeaveFootprint()
    {
        FeatureManager.Leave();

        _footprintLayer.DataHasChanged();
    }

    public void FlyToFootprint(string name)
    {
        var feature = _footprintProvider.Find(name, "Name")!;

        var center = feature.Extent.Centroid;
        var rect = feature.Extent.Grow(4.0);

        //var (x, y) = SphericalMercator.FromLonLat(center.X, center.Y);

        //mapService.FlyTo(new MPoint(x, y), 100000);

        // Map.Navigator.FlyTo(center, 100000, duration: 800);


        Map.Navigator.CenterOn(center, duration: 800);

        //  Map.Navigator.ZoomToBox(rect, duration: 800);

        //Observable.StartAsync(() => mapService.ForceUpdate(duration + 100), RxApp.MainThreadScheduler).Subscribe();


        //_footprintLayer.DataHasChanged();
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

    private static ILayer CreateSatelliteLayer(IProvider provider)
    {
        var style = StyleBuilder.CreateSatelliteLayerStyle();

        var layer = new DynamicLayer(provider, true)
        {
            Name = SatelliteKey,
            Style = style
        };

        return layer;
    }

    private static ILayer CreateGroundTargetLayer(IProvider provider)
    {
        var style = StyleBuilder.CreateGroundTargetLayerStyle();

        var layer = new DynamicLayer(provider, true)
        {
            Name = GroundTargetKey,
            Style = style
        };

        return layer;
    }

    private static ILayer CreateFootprintLayer(IProvider provider)
    {
        var style = StyleBuilder.CreateFootprintStyle(dimming: false);

        var layer = new Layer()
        {
            Name = FootprintKey,
            DataSource = provider,
            Style = style,
            IsMapInfoLayer = true,
        };

        return layer;
    }

    private static ILayer CreateFootprintPreviewLayer(IProvider provider)
    {
        var style = StyleBuilder.CreateFootprintTrackStyle();

        var layer = new DynamicLayer(provider, true)
        {
            Name = FootprintPreviewKey,
            //    DataSource = provider,
            Style = style,
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateTrackLayer()
    {
        var style = StyleBuilder.CreateTrackLayerStyle();

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
