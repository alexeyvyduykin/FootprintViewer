using FootprintViewer.Data;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using provider = Mapsui.Providers;

namespace FootprintViewer.Layers
{
    public interface ITargetLayerSource : provider.IProvider
    {
        ReactiveCommand<IEnumerable<IFeature>?, string[]?> Refresh { get; }

        ReactiveCommand<Unit, Unit> Init { get; }

        IFeature? GetFeature(string name);

        IList<IFeature> GetFeatures();

        double MinVisible { get; set; }

        double MaxVisible { get; set; }
    }

    public class TargetLayerSource : ITargetLayerSource
    {
        private MemoryProvider? _memoryProvider;
        private MRect? _lastExtent;
        private List<IFeature> _activeFeatures;

        public TargetLayerSource(IProvider<GroundTarget> provider)
        {
            Init = ReactiveCommand.CreateFromObservable(() => Observable.Start(() => LoadingImpl(provider.GetNativeValuesAsync(null).Result)));

            provider.Observable.Skip(1).Select(s => Unit.Default).InvokeCommand(Init);

            Refresh = ReactiveCommand.Create<IEnumerable<IFeature>?, string[]?>(s => RefreshImpl(s));

            _activeFeatures = new List<IFeature>();

            MinVisible = 0;

            MaxVisible = double.MaxValue;
        }

        public IList<IFeature> GetFeatures() => _activeFeatures.ToList();

        public ReactiveCommand<Unit, Unit> Init { get; }

        public ReactiveCommand<IEnumerable<IFeature>?, string[]?> Refresh { get; }

        public string? CRS { get; set; }

        public double MinVisible { get; set; }

        public double MaxVisible { get; set; }

        public IFeature? GetFeature(string name)
        {
            return _activeFeatures.Where(s => name.Equals((string)s["Name"]!)).FirstOrDefault();
        }

        public async Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
        {
            if (MinVisible > fetchInfo.Resolution || MaxVisible < fetchInfo.Resolution)
            {
                Refresh.Execute(null).Subscribe();

                return new List<IFeature>();
            }

            var res = await _memoryProvider!.GetFeaturesAsync(fetchInfo);

            if (fetchInfo.Extent.Equals(_lastExtent) == false)
            {
                _activeFeatures = res.ToList();

                Refresh.Execute(res).Subscribe();

                _lastExtent = fetchInfo.Extent;
            }

            return res;
        }

        public MRect? GetExtent() => _memoryProvider?.GetExtent();

        private static string[]? RefreshImpl(IEnumerable<IFeature>? features)
        {
            if (features == null)
            {
                return null;
            }

            return features.Where(s => s.Fields.Contains("Name")).Select(s => (string)s["Name"]!).ToArray();
        }

        protected void LoadingImpl(List<GroundTarget> groundTargets)
        {
            _memoryProvider = new MemoryProvider(Build(groundTargets));
            _memoryProvider.CRS = CRS;
        }

        private static List<IFeature> Build(IEnumerable<GroundTarget> groundTargets)
        {
            return groundTargets.Select(s =>
            {
                var geometry = s.Type switch
                {
                    GroundTargetType.Point => new Point(SphericalMercator.FromLonLat(((Point)s.Points!).X, ((Point)s.Points!).Y).ToCoordinate()),
                    GroundTargetType.Route => RouteCutting(s.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList()),
                    GroundTargetType.Area => AreaCutting(s.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList()),
                    _ => throw new Exception()
                };

                var feature = geometry.ToFeature();

                feature["Name"] = s.Name;
                feature["State"] = "Unselected";
                feature["Highlight"] = false;
                feature["Type"] = s.Type.ToString();

                return (IFeature)feature;
            }).ToList();
        }

        private static Geometry AreaCutting(IList<Point> points)
        {
            var count = points.Count;

            var vertices1 = new List<(double, double)>();
            var vertices2 = new List<(double, double)>();
            var vertices = vertices1;

            for (int i = 0; i < count; i++)
            {
                var p1 = points[i];
                var p2 = (i == count - 1) ? points[0] : points[i + 1];

                var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y);
                vertices.Add(point1);

                if (Math.Abs(p2.X - p1.X) > 180)
                {
                    if (p2.X - p1.X > 0) // -180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
                        vertices.Add(pp1);

                        vertices = (vertices == vertices1) ? vertices2 : vertices1;

                        var pp2 = SphericalMercator.FromLonLat(180, cutLat);
                        vertices.Add(pp2);
                    }

                    if (p2.X - p1.X < 0) // +180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(180, cutLat);
                        vertices.Add(pp1);

                        vertices = (vertices == vertices1) ? vertices2 : vertices1;

                        var pp2 = SphericalMercator.FromLonLat(-180, cutLat);
                        vertices.Add(pp2);
                    }
                }
            }

            if (vertices2.Count != 0) // multipolygon
            {
                var poly1 = new GeometryFactory().CreatePolygon(vertices1.ToClosedCoordinates());
                var poly2 = new GeometryFactory().CreatePolygon(vertices2.ToClosedCoordinates());

                return new GeometryFactory().CreateMultiPolygon(new[] { poly1, poly2 });
            }
            else
            {
                return new GeometryFactory().CreatePolygon(vertices1.ToClosedCoordinates());
            }
        }

        private static Geometry RouteCutting(IList<Point> points)
        {
            var count = points.Count;

            var vertices1 = new List<(double, double)>();
            var vertices2 = new List<(double, double)>();
            var vertices = vertices1;

            for (int i = 0; i < count - 1; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];

                var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y);
                vertices.Add(point1);

                if (Math.Abs(p2.X - p1.X) > 180)
                {
                    if (p2.X - p1.X > 0) // -180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
                        vertices.Add(pp1);

                        vertices = (vertices == vertices1) ? vertices2 : vertices1;

                        var pp2 = SphericalMercator.FromLonLat(180, cutLat);
                        vertices.Add(pp2);
                    }

                    if (p2.X - p1.X < 0) // +180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(180, cutLat);
                        vertices.Add(pp1);

                        vertices = (vertices == vertices1) ? vertices2 : vertices1;

                        var pp2 = SphericalMercator.FromLonLat(-180, cutLat);
                        vertices.Add(pp2);
                    }
                }
            }

            if (vertices2.Count != 0) // MultiLineString
            {
                var line1 = new LineString(vertices1.ToGreaterThanTwoCoordinates());
                var line2 = new LineString(vertices2.ToGreaterThanTwoCoordinates());

                return new GeometryFactory().CreateMultiLineString(new[] { line1, line2 });
            }
            else
            {
                return new LineString(vertices1.ToGreaterThanTwoCoordinates());
            }
        }

        private static double LinearInterpDiscontLat(Point p1, Point p2)
        {
            // one longitude should be negative one positive, make them both positive
            double lon1 = p1.X, lat1 = p1.Y, lon2 = p2.X, lat2 = p2.Y;
            if (lon1 > lon2)
            {
                lon2 += 360;
            }
            else
            {
                lon1 += 360;
            }

            return (lat1 + (180 - lon1) * (lat2 - lat1) / (lon2 - lon1));
        }
    }
}
