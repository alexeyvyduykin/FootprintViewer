using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer
{
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
            var source = dependencyResolver.GetExistingService<IEditLayerSource>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new BaseCustomLayer<IEditLayerSource>(source)
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
            var source = dependencyResolver.GetExistingService<IFootprintLayerSource>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var provider = dependencyResolver.GetExistingService<IProvider<Footprint>>();

            var layer = new FootprintLayer(source)
            {
                Style = styleManager.FootprintStyle,
                MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
                IsMapInfoLayer = true,
            };

            provider.Observable
                .Skip(1)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                var footprints = await provider.GetNativeValuesAsync(null);

                source.UpdateData(footprints);
            }
        }

        private static ILayer CreateGroundStationLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var loader = dependencyResolver.GetExistingService<TaskLoader>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<IGroundStationLayerSource>();
            var provider = dependencyResolver.GetExistingService<IProvider<GroundStation>>();

            var layer = new BaseCustomLayer<IGroundStationLayerSource>(source)
            {
                Style = styleManager.GroundStationStyle,
                IsMapInfoLayer = false,
            };

            provider.Observable
                .Skip(1)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                var groundStations = await provider.GetNativeValuesAsync(null);

                source.UpdateData(groundStations);
            }
        }

        private static ILayer CreateTrackLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var loader = dependencyResolver.GetExistingService<TaskLoader>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ITrackLayerSource>();
            var provider = dependencyResolver.GetExistingService<IProvider<Satellite>>();

            var layer = new BaseCustomLayer<ITrackLayerSource>(source)
            {
                Style = styleManager.TrackStyle,
                IsMapInfoLayer = false,
            };

            provider.Observable
                .Skip(1)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                var satellites = await provider.GetNativeValuesAsync(null);

                source.UpdateData(satellites);
            }
        }

        private static ILayer CreateTargetLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var loader = dependencyResolver.GetExistingService<TaskLoader>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ITargetLayerSource>();
            var provider = dependencyResolver.GetExistingService<IProvider<GroundTarget>>();

            source.MaxVisible = styleManager.MaxVisibleTargetStyle;

            var layer = new TargetLayer()
            {
                Style = styleManager.TargetStyle,
                //MaxVisible = styleManager.MaxVisibleTargetStyle,
                DataSource = source,
                IsMapInfoLayer = true,
            };

            provider.Observable
                .Skip(1)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                var groundTargets = await provider.GetNativeValuesAsync(null);

                source.SetProvider(new MemoryProvider(FeatureBuilder.Build(groundTargets)));

                layer.ClearCache();

                layer.DataSource = null;

                layer.DataSource = source;
            }
        }

        private static ILayer CreateSensorLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var loader = dependencyResolver.GetExistingService<TaskLoader>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ISensorLayerSource>();
            var provider = dependencyResolver.GetExistingService<IProvider<Satellite>>();

            var layer = new BaseCustomLayer<ISensorLayerSource>(source)
            {
                Style = styleManager.SensorStyle,
                IsMapInfoLayer = false,
            };

            provider.Observable
                .Skip(1)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                var satellites = await provider.GetNativeValuesAsync(null);

                source.UpdateData(satellites);
            }
        }

        private static ILayer CreateUserLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var loader = dependencyResolver.GetExistingService<TaskLoader>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<IUserLayerSource>();
            var editableProvider = dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();

            var layer = new BaseCustomLayer<IUserLayerSource>(source)
            {
                IsMapInfoLayer = true,
                Style = styleManager.UserStyle,
            };

            editableProvider.Observable
                .Skip(1)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                var userGeometries = await editableProvider.GetNativeValuesAsync(null);

                source.UpdateData(userGeometries);
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
}
