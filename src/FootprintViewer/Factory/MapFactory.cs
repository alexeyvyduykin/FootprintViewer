using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer;

public class MapFactory
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;

    public MapFactory(IReadonlyDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;
    }

    public Map CreateMap()
    {
        var map = new Map()
        {
            CRS = "EPSG:3857",
            //   Transformation = new MinimalTransformation(),
        };

        map.AddLayer(new Layer(), LayerType.WorldMap);
        map.AddLayer(new WritableLayer(), LayerType.FootprintImage);
        map.AddLayer(CreateGroundStationLayer(_dependencyResolver), LayerType.GroundStation);
        map.AddLayer(CreateTargetLayer(_dependencyResolver), LayerType.GroundTarget);
        map.AddLayer(CreateSensorLayer(_dependencyResolver), LayerType.Sensor);
        map.AddLayer(CreateTrackLayer(_dependencyResolver), LayerType.Track);
        map.AddLayer(CreateFootprintLayer(_dependencyResolver), LayerType.Footprint);
        map.AddLayer(CreateFootprintImageBorderLayer(_dependencyResolver), LayerType.FootprintImageBorder);
        map.AddLayer(CreateEditLayer(_dependencyResolver), LayerType.Edit);
        map.AddLayer(CreateVertexOnlyLayer(map, _dependencyResolver), LayerType.Vertex);
        map.AddLayer(CreateUserLayer(_dependencyResolver), LayerType.User);

        return map;
    }

    private static ILayer CreateEditLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

        return new EditLayer()
        {
            Style = styleManager.EditStyle,
            IsMapInfoLayer = false,
        };
    }

    private static ILayer CreateVertexOnlyLayer(Map map, IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var editLayer = map.GetLayer(LayerType.Edit);

        return new VertexOnlyLayer(editLayer!)
        {
            Style = styleManager.VertexOnlyStyle,
        };
    }

    private static ILayer CreateFootprintLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var loader = dependencyResolver.GetExistingService<TaskLoader>();
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var dataManager = dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var layer = new WritableLayer()
        {
            Style = styleManager.FootprintStyle,
            //   MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
            IsMapInfoLayer = true,
        };

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync, outputScheduler: RxApp.MainThreadScheduler));

        loader.AddTaskAsync(() => LoadingAsync());

        return layer;

        async Task LoadingAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            var footprints = await dataManager.GetDataAsync<Footprint>(DbKeys.Footprints.ToString());

            layer.Clear();
            layer.AddRange(FeatureBuilder.Build(footprints));
            layer.DataHasChanged();
        }
    }

    private static ILayer CreateGroundStationLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var loader = dependencyResolver.GetExistingService<TaskLoader>();
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var dataManager = dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var layer = new GroundStationLayer()
        {
            Style = styleManager.GroundStationStyle,
            IsMapInfoLayer = false,
        };

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync, outputScheduler: RxApp.MainThreadScheduler));

        loader.AddTaskAsync(() => LoadingAsync());

        return layer;

        async Task LoadingAsync()
        {
            var groundStations = await dataManager.GetDataAsync<GroundStation>(DbKeys.GroundStations.ToString());

            layer.UpdateData(groundStations);
        }
    }

    private static ILayer CreateTrackLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var loader = dependencyResolver.GetExistingService<TaskLoader>();
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var dataManager = dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var layer = new TrackLayer()
        {
            Style = styleManager.TrackStyle,
            IsMapInfoLayer = false,
        };

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync, outputScheduler: RxApp.MainThreadScheduler));

        loader.AddTaskAsync(() => LoadingAsync());

        return layer;

        async Task LoadingAsync()
        {
            var satellites = await dataManager.GetDataAsync<Satellite>(DbKeys.Satellites.ToString());

            layer.UpdateData(satellites);
        }
    }

    private static ILayer CreateTargetLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var loader = dependencyResolver.GetExistingService<TaskLoader>();
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var source = dependencyResolver.GetExistingService<ITargetLayerSource>();
        var dataManager = dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        source.MaxVisible = styleManager.MaxVisibleTargetStyle;

        var layer = new Layer()
        {
            Style = styleManager.TargetStyle,
            //MaxVisible = styleManager.MaxVisibleTargetStyle,
            DataSource = source,
            IsMapInfoLayer = true,
        };

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync, outputScheduler: RxApp.MainThreadScheduler));

        loader.AddTaskAsync(() => LoadingAsync());

        return layer;

        async Task LoadingAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            var groundTargets = await dataManager.GetDataAsync<GroundTarget>(DbKeys.GroundTargets.ToString());

            source.SetProvider(new MemoryProvider(FeatureBuilder.Build(groundTargets)));

            layer.ClearCache();

            layer.DataSource = null;

            layer.DataSource = source;

            layer.DataHasChanged();
        }
    }

    private static ILayer CreateSensorLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var loader = dependencyResolver.GetExistingService<TaskLoader>();
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var dataManager = dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var layer = new SensorLayer()
        {
            Style = styleManager.SensorStyle,
            IsMapInfoLayer = false,
        };

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync, outputScheduler: RxApp.MainThreadScheduler));


        loader.AddTaskAsync(() => LoadingAsync());

        return layer;

        async Task LoadingAsync()
        {
            var satellites = await dataManager.GetDataAsync<Satellite>(DbKeys.Satellites.ToString());

            layer.UpdateData(satellites);
        }
    }

    private static ILayer CreateUserLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var loader = dependencyResolver.GetExistingService<TaskLoader>();
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var editableProvider = dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();

        var layer = new WritableLayer()
        {
            IsMapInfoLayer = true,
            Style = styleManager.UserStyle,
        };

        var skip = editableProvider.Sources.Count > 0 ? 1 : 0;

        editableProvider.Observable
            .Skip(skip)
            .Select(s => Unit.Default)
            .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

        editableProvider.Update.InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync2));

        loader.AddTaskAsync(() => LoadingAsync());

        return layer;

        async Task LoadingAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            var userGeometries = await editableProvider.GetNativeValuesAsync(null);

            var arr = userGeometries
                .Where(s => s.Geometry != null)
                .Select(s => s.Geometry!.ToFeature(s.Name!));

            layer.Clear();
            layer.AddRange(arr);
            layer.DataHasChanged();
        }

        async Task LoadingAsync2()
        {
            var userGeometries = await editableProvider.GetNativeValuesAsync(null);

            var arr = userGeometries
                .Where(s => s.Geometry != null)
                .Select(s => s.Geometry!.ToFeature(s.Name!));

            layer.Clear();
            layer.AddRange(arr);
            layer.DataHasChanged();
        }
    }

    private static ILayer CreateFootprintImageBorderLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

        return new WritableLayer
        {
            Style = styleManager.FootprintImageBorderStyle,
        };
    }
}
