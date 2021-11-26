#nullable enable
using DatabaseCreatorSample.Data;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelliteGeometrySample
{
    public class FootprintProvider : MemoryProvider
    {
        private IFeature _lastSelected;
        private readonly Dictionary<string, List<IFeature>> _dict = new Dictionary<string, List<IFeature>>();        
        private readonly Dictionary<string, List<Footprint>> _cache = new Dictionary<string, List<Footprint>>();

        public FootprintProvider()
        {
            _dict = Build();

            ReplaceFeatures(_dict.SelectMany(s => s.Value));
        }

        public Footprint GetFootprint(string name)
        {
            return _cache.SelectMany(s => s.Value).Where(s => s.Name.Equals(name)).FirstOrDefault();
        }

        public IEnumerable<Footprint> GetFootprints()
        {
            return _cache.SelectMany(s => s.Value);
        }

        public void SelectFeature(string name)
        {
            var feature = _dict.SelectMany(s => s.Value).Where(s => name.Equals((string)s["Name"])).First();

            if (_lastSelected != null)
            {
                _lastSelected["State"] = "Unselect";
            }

            feature["State"] = "Select";

            _lastSelected = feature;
        }

        public void UnselectFeature(string name)
        {
            if (_lastSelected != null && name.Equals(_lastSelected["Name"]) == true)
            {
                _lastSelected["State"] = "Unselect";
            }
        }

        private Dictionary<string, List<IFeature>> Build()
        {
            var dictionary = new Dictionary<string, List<IFeature>>();

            foreach (var satellite in SatelliteDataSource.Satellites.Take(1))
            {
                FootprintDataSource source = new FootprintDataSource(satellite);

                var list = new List<IFeature>();

                foreach (var item in source.Footprints)
                {
                    //var ring = new LinearRing();

                    //foreach (var p in item.Border)
                    //{
                    //    var point = SphericalMercator.FromLonLat(p.X, p.Y);
                    //    ring.Vertices.Add(point);
                    //}

                    //var poly = new Polygon() { ExteriorRing = ring };

                    var poly = AreaCutting(item.Border.ToList());

                    var feature = new Feature { Geometry = poly };
                    
                    feature["Name"] = item.Name;
                    feature["State"] = "Unselect";

                    list.Add(feature);
                }

                dictionary.Add(satellite.Name, list);

                _cache.Add(satellite.Name, source.Footprints.ToList());
            }

            return dictionary;
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
                        var cutLat = linearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
                        ring.Vertices.Add(pp1);

                        ring = (ring == ring1) ? ring2 : ring1;

                        var pp2 = SphericalMercator.FromLonLat(180, cutLat);
                        ring.Vertices.Add(pp2);
                    }

                    if (p2.X - p1.X < 0) // +180 cutting
                    {
                        var cutLat = linearInterpDiscontLat(p1, p2);
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

        private double linearInterpDiscontLat(Point p1, Point p2)
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
