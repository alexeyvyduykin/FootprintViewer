using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using NetTopologySuite.Index.HPRtree;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using nts = NetTopologySuite.Geometries;

namespace FootprintViewer.Layers
{
    public class CustomProvider : WritableLayer
    {
        private readonly UserGeometryProvider _provider;

        public CustomProvider(UserGeometryProvider provider)
        {
            _provider = provider;

            Update = ReactiveCommand.CreateFromTask(UpdateAsync);

            Loading = ReactiveCommand.Create<List<UserGeometryInfo>>(LoadingImpl);

            Update.Select(s => s).InvokeCommand(Loading);

            provider.Update.Select(_ => Unit.Default).InvokeCommand(Update);

            provider.Loading.Select(s => s).InvokeCommand(Loading);
        }

        private ReactiveCommand<Unit, List<UserGeometryInfo>> Update { get; }

        private ReactiveCommand<List<UserGeometryInfo>, Unit> Loading { get; }

        private string GenerateName(UserGeometryType type)
        {
            return $"{type}_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}";
        }

        private void LoadingImpl(List<UserGeometryInfo> userGeometries)
        {
            var arr = userGeometries
                .Where(s => s.Geometry != null)
                .Select(s => new Feature() { Geometry = NTSConverter.ToPolygon(s.Geometry.Geometry!) });
            
            Clear();
            AddRange(arr);
        }

        private async Task<List<UserGeometryInfo>> UpdateAsync()
        {
            return await _provider.GetUserGeometryInfosAsync();
        }

        public void AddFeature(IFeature feature)
        {
            Add(feature);
        }

        public void AddRectangle(IFeature feature)
        {
            Add(feature);

            Task.Run(async () =>
            {
                var model = new UserGeometry()
                {
                    Type = UserGeometryType.Rectangle,
                    Name = GenerateName(UserGeometryType.Rectangle),
                    Geometry = NTSConverter.FromPolygon((Polygon)feature.Geometry)
                };

                await _provider.AddAsync(model);
            });
        }

        public void AddCircle(IFeature feature)
        {
            Add(feature);

            Task.Run(async () =>
            {
                var model = new UserGeometry()
                {
                    Type = UserGeometryType.Circle,
                    Name = GenerateName(UserGeometryType.Circle),
                    Geometry = NTSConverter.FromPolygon((Polygon)feature.Geometry)
                };

                await _provider.AddAsync(model);
            });
        }

        public void AddPolygon(IFeature feature)
        {
            Add(feature);

            Task.Run(async () =>
            {
                var model = new UserGeometry()
                {
                    Type = UserGeometryType.Polygon,
                    Name = GenerateName(UserGeometryType.Polygon),
                    Geometry = NTSConverter.FromPolygon((Polygon)feature.Geometry)
                };

                await _provider.AddAsync(model);
            });
        }
    }

    public static class NTSConverter
    {
        public static nts.Geometry FromPolygon(Polygon polygon)
        {
            var points = polygon.ExteriorRing.Vertices;
            var lr = new nts.LinearRing(points.Select(s => new nts.Coordinate(s.X, s.Y)).ToArray());
            return new nts.Polygon(lr);
        }

        public static Polygon ToPolygon(nts.Geometry geometry)
        {
            var points = geometry.Coordinates;

            var lr = new LinearRing(points.Select(s => new Point(s.X, s.Y)).ToArray());
            return new Polygon(lr);
        }
    }
}
