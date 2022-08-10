﻿using FootprintViewer.Data;
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
            var provider = dependencyResolver.GetExistingService<IProvider<Footprint>>();

            var layer = new WritableLayer()
            {
                Style = styleManager.FootprintStyle,
                //   MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
                IsMapInfoLayer = true,
            };

            var skip = provider.Sources.Count > 0 ? 1 : 0;

            provider.Observable
                .Skip(skip)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                var footprints = await provider.GetNativeValuesAsync(null);

                layer.Clear();
                layer.AddRange(FeatureBuilder.Build(footprints));
                layer.DataHasChanged();
            }
        }

        private static ILayer CreateGroundStationLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var loader = dependencyResolver.GetExistingService<TaskLoader>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var provider = dependencyResolver.GetExistingService<IProvider<GroundStation>>();

            var layer = new GroundStationLayer()
            {
                Style = styleManager.GroundStationStyle,
                IsMapInfoLayer = false,
            };

            var skip = provider.Sources.Count > 0 ? 1 : 0;

            provider.Observable
                .Skip(skip)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                var groundStations = await provider.GetNativeValuesAsync(null);

                layer.UpdateData(groundStations);
            }
        }

        private static ILayer CreateTrackLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var loader = dependencyResolver.GetExistingService<TaskLoader>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var provider = dependencyResolver.GetExistingService<IProvider<Satellite>>();

            var layer = new TrackLayer()
            {
                Style = styleManager.TrackStyle,
                IsMapInfoLayer = false,
            };

            var skip = provider.Sources.Count > 0 ? 1 : 0;

            provider.Observable
                .Skip(skip)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                var satellites = await provider.GetNativeValuesAsync(null);

                layer.UpdateData(satellites);
            }
        }

        private static ILayer CreateTargetLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var loader = dependencyResolver.GetExistingService<TaskLoader>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ITargetLayerSource>();
            var provider = dependencyResolver.GetExistingService<IProvider<GroundTarget>>();

            source.MaxVisible = styleManager.MaxVisibleTargetStyle;

            var layer = new Layer()
            {
                Style = styleManager.TargetStyle,
                //MaxVisible = styleManager.MaxVisibleTargetStyle,
                DataSource = source,
                IsMapInfoLayer = true,
            };

            var skip = provider.Sources.Count > 0 ? 1 : 0;

            provider.Observable
                .Skip(skip)
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
            var provider = dependencyResolver.GetExistingService<IProvider<Satellite>>();

            var layer = new SensorLayer()
            {
                Style = styleManager.SensorStyle,
                IsMapInfoLayer = false,
            };

            var skip = provider.Sources.Count > 0 ? 1 : 0;

            provider.Observable
                .Skip(skip)
                .Select(s => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync));

            loader.AddTaskAsync(() => LoadingAsync());

            return layer;

            async Task LoadingAsync()
            {
                var satellites = await provider.GetNativeValuesAsync(null);

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
}
