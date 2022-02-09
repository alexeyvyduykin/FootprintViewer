using FootprintViewer.Data;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
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

            provider.Update.Subscribe(_ => Update());

            provider.Loading.Subscribe(LoadingImpl);
        }

        private void LoadingImpl(IEnumerable<UserGeometry> userGeometries)
        {                   
            foreach (var item in userGeometries)
            {
                if (item.Geometry != null)
                {
                    Add(new Feature() { Geometry = NTSConverter.ToPolygon(item.Geometry) });
                }
            }
        }

        private void Update()
        {
            Clear();

            var res = _provider.LoadUsers();

            foreach (var item in res)
            {
                if (item.Geometry != null)
                {
                    Add(new Feature() { Geometry = NTSConverter.ToPolygon(item.Geometry) });
                }
            }
        }

        private async void UpdateImpl()
        {
            Clear();

            var res = await _provider.LoadUsersAsync();

            foreach (var item in res)
            {
                if (item.Geometry != null)
                {
                    Add(new Feature() { Geometry = NTSConverter.ToPolygon(item.Geometry) });
                }
            }
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
                    Name = $"Rectangle_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}",
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
                    Name = $"Circle_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}",
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
                    Name = $"Polygon_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}",
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
