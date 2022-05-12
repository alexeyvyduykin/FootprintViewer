using FootprintViewer.Data;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers
{
    public interface IFootprintLayerSource : ILayer
    {
        IEnumerable<IFeature> GetFeatures();

        IFeature? GetFeature(string name);
    }

    public class FootprintLayerSource : WritableLayer, IFootprintLayerSource
    {
        private readonly FootprintProvider _provider;

        public FootprintLayerSource(FootprintProvider provider)
        {
            _provider = provider;

            Loading = ReactiveCommand.Create<List<Footprint>>(LoadingImpl);

            provider.Loading.Select(s => s).InvokeCommand(Loading);
        }

        private ReactiveCommand<List<Footprint>, Unit> Loading { get; }

        private void LoadingImpl(List<Footprint> footprints)
        {
            Clear();
            AddRange(Build(footprints));
        }

        public Footprint GetFootprint(string name) => _provider.GetFootprints().Where(s => s.Name!.Equals(name)).FirstOrDefault()!;

        public async Task<List<Footprint>> GetFootprintsAsync() => await Task.Run(() => _provider.GetFootprints().ToList());

        public List<Footprint> GetFootprints() => _provider.GetFootprints().ToList();

        public IFeature? GetFeature(string name) => GetFeatures().Where(s => s.Fields.Contains("Name") && s["Name"]!.Equals(name)).FirstOrDefault();

        private static List<IFeature> Build(IEnumerable<Footprint> footprints)
        {
            var list = new List<IFeature>();

            foreach (var item in footprints)
            {
                var poly = AreaCutting(item.Points!.Coordinates);

                var feature = poly.ToFeature(item.Name!);

                list.Add(feature);
            }

            return list;
        }

        //private IGeometry AreaCutting(IList<Point> points)
        //{
        //    var count = points.Count;
        //    var ring1 = new LinearRing();
        //    var ring2 = new LinearRing();

        //    var ring = ring1;

        //    for (int i = 0; i < count; i++)
        //    {
        //        var p1 = points[i];
        //        var p2 = (i == count - 1) ? points[0] : points[i + 1];

        //        var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y);
        //        ring.Vertices.Add(point1);

        //        if (Math.Abs(p2.X - p1.X) > 180)
        //        {
        //            if (p2.X - p1.X > 0) // -180 cutting
        //            {
        //                var cutLat = LinearInterpDiscontLat(p1, p2);
        //                var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
        //                ring.Vertices.Add(pp1);

        //                ring = (ring == ring1) ? ring2 : ring1;

        //                var pp2 = SphericalMercator.FromLonLat(180, cutLat);
        //                ring.Vertices.Add(pp2);
        //            }

        //            if (p2.X - p1.X < 0) // +180 cutting
        //            {
        //                var cutLat = LinearInterpDiscontLat(p1, p2);
        //                var pp1 = SphericalMercator.FromLonLat(180, cutLat);
        //                ring.Vertices.Add(pp1);

        //                ring = (ring == ring1) ? ring2 : ring1;

        //                var pp2 = SphericalMercator.FromLonLat(-180, cutLat);
        //                ring.Vertices.Add(pp2);
        //            }
        //        }


        //    }


        //    if (ring2.Length != 0) // multipolygon
        //    {
        //        var multi = new MultiPolygon();
        //        multi.Polygons.Add(new Polygon() { ExteriorRing = ring1 });
        //        multi.Polygons.Add(new Polygon() { ExteriorRing = ring2 });

        //        return multi;
        //    }
        //    else
        //    {
        //        return new Polygon() { ExteriorRing = ring1 };
        //    }
        //}

        private static Geometry AreaCutting(Coordinate[] points)
        {
            var count = points.Length;
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

        private static double LinearInterpDiscontLat(Coordinate p1, Coordinate p2)
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
