using FootprintViewer.Data;
using Mapsui;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public interface IFootprintLayerSource : ILayerSource
    {

    }

    public class FootprintLayerSource : BaseLayerSource<Footprint>, IFootprintLayerSource
    {
        public FootprintLayerSource(IProvider<Footprint> provider) : base(provider) { }

        protected override void LoadingImpl(List<Footprint> footprints)
        {
            Clear();
            AddRange(Build(footprints));
            DataHasChanged();
        }

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
