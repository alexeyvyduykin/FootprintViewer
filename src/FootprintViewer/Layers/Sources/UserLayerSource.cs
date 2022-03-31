using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
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
    public interface IUserLayerSource : ILayer
    {
        void EditFeature(IFeature feature);

        void AddUserGeometry(IFeature feature, UserGeometryType type);
    }

    public class UserLayerSource : WritableLayer, IUserLayerSource
    {
        private readonly UserGeometryProvider _provider;

        public UserLayerSource(UserGeometryProvider provider)
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
                .Select(s => 
                {
                    if (s.Type == UserGeometryType.Point)
                    {
                        return new Feature()
                        {
                            ["Name"] = s.Name,
                            Geometry = NTSConverter.ToPoint(s.Geometry.Geometry!)
                        };
                    }
                    else
                    {
                        return new Feature()
                        {
                            ["Name"] = s.Name,
                            Geometry = NTSConverter.ToPolygon(s.Geometry.Geometry!)
                        };
                    }                                
                });

            Clear();
            AddRange(arr);
        }

        private async Task<List<UserGeometryInfo>> UpdateAsync()
        {
            return await _provider.GetUserGeometryInfosAsync();
        }

        public void EditFeature(IFeature feature)
        {
            Task.Run(async () =>
            {
                if (feature.Fields.Contains("Name") == true)
                {
                    var name = (string)feature["Name"];

                    var geometry = NTSConverter.FromGeometry(feature.Geometry);

                    await _provider.UpdateGeometry(name, geometry);
                }
            });
        }

        public void AddUserGeometry(IFeature feature, UserGeometryType type)
        {
            var name = GenerateName(type);

            feature["Name"] = name;

            Add(feature);

            Task.Run(async () =>
            {
                var model = new UserGeometry()
                {
                    Type = type,
                    Name = name,
                    Geometry = NTSConverter.FromGeometry(feature.Geometry)
                };

                await _provider.AddAsync(model);
            });
        }
    }

    public static class NTSConverter
    {
        private static nts.Geometry FromPoint(Point point)
        {           
            return new nts.Point(new nts.Coordinate(point.X, point.Y));
        }

        public static Point ToPoint(nts.Geometry geometry)
        {
            var coordinate = ((nts.Point)geometry).Coordinate;
            return new Point(coordinate.X, coordinate.Y);
        }

        private static nts.Geometry FromPolygon(Polygon polygon)
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

        public static nts.Geometry FromGeometry(IGeometry geometry)
        {
            if (geometry is Point point)
            {
                return FromPoint(point);
            }
            else if (geometry is Polygon polygon)
            {
                return FromPolygon(polygon);
            }

            throw new Exception();
        }
    }
}
