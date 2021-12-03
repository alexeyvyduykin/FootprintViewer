using FootprintViewer.Data;
using FootprintViewer.Data.Science;
using FootprintViewer.ViewModels;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Providers;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public class SensorProvider : MemoryProvider
    {
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dictLeft = new Dictionary<string, Dictionary<int, List<IFeature>>>();
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dictright = new Dictionary<string, Dictionary<int, List<IFeature>>>();
        private readonly Dictionary<string, List<IFeature>> _cache = new Dictionary<string, List<IFeature>>();

        public SensorProvider(IDataSource source)
        {
            var satellites = source.LeftStrips.Keys;

            foreach (var name in satellites)
            {                             
                var dictLeft = FromStrips(source.LeftStrips[name]);
                var dictRight = FromStrips(source.RightStrips[name]);

                _dictLeft.Add(name, dictLeft);
                _dictright.Add(name, dictRight);

                if (_cache.ContainsKey(name) == false)
                {
                    _cache.Add(name, new List<IFeature>());
                }
            }
        }

        public void Update(SatelliteInfo info)
        {
            var name = info.Name;

            _cache[name].Clear();

            if (info.IsShow == true)
            {
                var node = info.CurrentNode;

                if (info.IsLeftStrip == true)
                {
                    if (_dictLeft.ContainsKey(name) == true && _dictLeft[name].ContainsKey(node) == true)
                    {
                        var features = _dictLeft[name][node];
                        _cache[name].AddRange(features);
                    }
                }

                if (info.IsRightStrip == true)
                {
                    if (_dictright.ContainsKey(name) == true && _dictright[name].ContainsKey(node) == true)
                    {
                        var features = _dictright[name][node];
                        _cache[name].AddRange(features);
                    }
                }
            }

            ReplaceFeatures(_cache.SelectMany(s => s.Value));
        }

        private Dictionary<int, List<IFeature>> FromStrips(Dictionary<int, List<List<NetTopologySuite.Geometries.Point>>> strips)
        {
            var dict = new Dictionary<int, List<IFeature>>();
            foreach (var item in strips)
            {
                var list = new List<IFeature>();

                foreach (var ln in item.Value)
                {
                    var ring = new LinearRing();

                    foreach (var p in ln)
                    {
                        var point = SphericalMercator.FromLonLat(p.X * ScienceMath.RadiansToDegrees, p.Y * ScienceMath.RadiansToDegrees);
                        ring.Vertices.Add(point);
                    }

                    var poly = new Polygon() { ExteriorRing = ring };

                    list.Add(new Feature { Geometry = poly });
                }

                dict.Add(item.Key, list);
            }

            return dict;
        }
    }
}
