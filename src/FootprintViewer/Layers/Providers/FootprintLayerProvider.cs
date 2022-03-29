using FootprintViewer.Data;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers
{
    public class FootprintLayerProvider : MemoryProvider
    {
        private IFeature? _lastSelected;
        private List<IFeature>? _featuresCache;
        private readonly FootprintProvider _provider;

        public FootprintLayerProvider(FootprintProvider provider)
        {
            _provider = provider;

            provider.Loading.Subscribe(LoadingImpl);
        }

        private void LoadingImpl(List<Footprint> footprints)
        {
            _featuresCache = Build(footprints);

            ReplaceFeatures(_featuresCache);
        }

        public Footprint GetFootprint(string name)
        {
            return _provider.GetFootprints().Where(s => s.Name!.Equals(name)).FirstOrDefault();
        }

        public async Task<List<Footprint>> GetFootprintsAsync()
        {
            return await Task.Run(() => _provider.GetFootprints().ToList());
        }

        public List<Footprint> GetFootprints() => _provider.GetFootprints().ToList();

        public void SelectFeature(string name)
        {
            var feature = _featuresCache.Where(s => name.Equals((string)s["Name"])).First();

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

        public bool IsSelect(string name)
        {
            var feature = _featuresCache.Where(s => name.Equals((string)s["Name"])).First();

            if (feature != null)
            {
                return (string)feature["State"] == "Select";
            }

            throw new Exception();
        }

        private List<IFeature> Build(IEnumerable<Footprint> footprints)
        {
            var list = new List<IFeature>();

            foreach (var item in footprints)
            {
                var poly = AreaCutting(item.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList());

                var feature = new Feature { Geometry = poly };

                feature["Name"] = item.Name;
                feature["State"] = "Unselect";

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
