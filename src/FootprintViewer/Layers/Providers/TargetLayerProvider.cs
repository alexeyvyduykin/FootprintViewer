using FootprintViewer.Data;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public class TargetLayerProvider : MemoryProvider
    {
        private List<IFeature>? _featuresCache;

        public TargetLayerProvider(GroundTargetProvider provider)
        {
            provider.Loading.Subscribe(LoadingImpl);
        }

        private void LoadingImpl(List<GroundTarget> groundTargets)
        {
            _featuresCache = Build(groundTargets);

            ReplaceFeatures(_featuresCache);
        }

        public List<IFeature> FeaturesCache => _featuresCache!;

        private List<IFeature> Build(IEnumerable<GroundTarget> groundTargets)
        {
            List<IFeature> list = new List<IFeature>();

            foreach (var item in groundTargets)
            {
                var feature = new Feature()
                {
                    ["Name"] = item.Name,
                    ["State"] = "Unselected",
                    ["Highlight"] = false,
                };

                switch (item.Type)
                {
                    case GroundTargetType.Point:
                    {
                        var p = (NetTopologySuite.Geometries.Point)item.Points;
                        var point = SphericalMercator.FromLonLat(p.X, p.Y);
                        feature.Geometry = point;
                        feature["Type"] = "Point";
                    }
                    break;
                    case GroundTargetType.Route:
                    {
                        feature.Geometry = RouteCutting(item.Points.Coordinates.Select(s => new Point(s.X, s.Y)).ToList());
                        feature["Type"] = "Route";
                    }
                    break;
                    case GroundTargetType.Area:
                    {
                        feature.Geometry = AreaCutting(item.Points.Coordinates.Select(s => new Point(s.X, s.Y)).ToList());
                        feature["Type"] = "Area";
                    }
                    break;
                    default:
                        throw new Exception();
                }

                list.Add(feature);
            }

            return list;
        }

        private IGeometry AreaCutting(IList<Point> points)
        {
            var count = points.Count;
            var ring1 = new LinearRing();
            var ring2 = new LinearRing();

            var ring = ring1;

            for (int i = 0; i < count; i++)
            {
                var p1 = points[i];
                var p2 = (i == count - 1) ? points[0] : points[i + 1];

                var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y);
                ring.Vertices.Add(point1);

                if (Math.Abs(p2.X - p1.X) > 180)
                {
                    if (p2.X - p1.X > 0) // -180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
                        ring.Vertices.Add(pp1);

                        ring = (ring == ring1) ? ring2 : ring1;

                        var pp2 = SphericalMercator.FromLonLat(180, cutLat);
                        ring.Vertices.Add(pp2);
                    }

                    if (p2.X - p1.X < 0) // +180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(180, cutLat);
                        ring.Vertices.Add(pp1);

                        ring = (ring == ring1) ? ring2 : ring1;

                        var pp2 = SphericalMercator.FromLonLat(-180, cutLat);
                        ring.Vertices.Add(pp2);
                    }
                }


            }


            if (ring2.Length != 0) // multipolygon
            {
                var multi = new MultiPolygon();
                multi.Polygons.Add(new Polygon() { ExteriorRing = ring1 });
                multi.Polygons.Add(new Polygon() { ExteriorRing = ring2 });

                return multi;
            }
            else
            {
                return new Polygon() { ExteriorRing = ring1 };
            }
        }

        private IGeometry RouteCutting(IList<Point> points)
        {
            var count = points.Count;
            var line1 = new LineString();
            var line2 = new LineString();

            var line = line1;

            for (int i = 0; i < count - 1; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];

                var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y);
                line.Vertices.Add(point1);

                if (Math.Abs(p2.X - p1.X) > 180)
                {
                    if (p2.X - p1.X > 0) // -180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
                        line.Vertices.Add(pp1);

                        line = (line == line1) ? line2 : line1;

                        var pp2 = SphericalMercator.FromLonLat(180, cutLat);
                        line.Vertices.Add(pp2);
                    }

                    if (p2.X - p1.X < 0) // +180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(180, cutLat);
                        line.Vertices.Add(pp1);

                        line = (line == line1) ? line2 : line1;

                        var pp2 = SphericalMercator.FromLonLat(-180, cutLat);
                        line.Vertices.Add(pp2);
                    }
                }


            }


            if (line2.Length != 0) // MultiLineString
            {
                var multi = new MultiLineString();
                multi.LineStrings.Add(line1);
                multi.LineStrings.Add(line2);

                return multi;
            }
            else
            {
                return line1;
            }
        }

        private double LinearInterpDiscontLat(Point p1, Point p2)
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
