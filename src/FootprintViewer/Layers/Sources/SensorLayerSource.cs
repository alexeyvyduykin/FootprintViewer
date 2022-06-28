using FootprintViewer.Data;
using FootprintViewer.Data.Science;
using Mapsui;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public interface ISensorLayerSource : ILayerSource
    {
        void Update(string name, int node, bool isShowLeft, bool isShowRight);
    }

    public class SensorLayerSource : BaseLayerSource<Satellite>, ISensorLayerSource
    {
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dictLeft;
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dictright;
        private readonly Dictionary<string, List<IFeature>> _cache;

        public SensorLayerSource(IProvider<Satellite> provider) : base(provider)
        {
            _cache = new Dictionary<string, List<IFeature>>();

            _dictLeft = new Dictionary<string, Dictionary<int, List<IFeature>>>();

            _dictright = new Dictionary<string, Dictionary<int, List<IFeature>>>();
        }

        protected override void LoadingImpl(List<Satellite> satellites)
        {
            var leftStrips = StripBuilder.CreateLeft(satellites);
            var rightStrips = StripBuilder.CreateRight(satellites);

            _dictLeft.Clear();
            _dictright.Clear();

            _cache.Clear();

            foreach (var sat in satellites)
            {
                var name = sat.Name!;
                var dictLeft = FromStrips(name, leftStrips[name]);
                var dictRight = FromStrips(name, rightStrips[name]);

                _dictLeft.Add(name, dictLeft);
                _dictright.Add(name, dictRight);

                if (_cache.ContainsKey(name) == false)
                {
                    _cache.Add(name, new List<IFeature>());
                }
            }
        }

        public void Update(string name, int node, bool isShowLeft, bool isShowRight)
        {
            if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
            {
                _cache[name].Clear();

                if (isShowLeft == true)
                {
                    if (_dictLeft.ContainsKey(name) == true && _dictLeft[name].ContainsKey(node) == true)
                    {
                        var features = _dictLeft[name][node];
                        _cache[name].AddRange(features);
                    }
                }

                if (isShowRight == true)
                {
                    if (_dictright.ContainsKey(name) == true && _dictright[name].ContainsKey(node) == true)
                    {
                        var features = _dictright[name][node];
                        _cache[name].AddRange(features);
                    }
                }

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
                DataHasChanged();
            }
        }

        private static Dictionary<int, List<IFeature>> FromStrips(string name, Dictionary<int, List<List<(double lon, double lat)>>> strips)
        {
            var dict = new Dictionary<int, List<IFeature>>();

            foreach (var item in strips)
            {
                var list = item.Value.Select(s =>
                {
                    var vertices = s.Select(s => SphericalMercator.FromLonLat(s.lon * ScienceMath.RadiansToDegrees, s.lat * ScienceMath.RadiansToDegrees));

                    var poly = new GeometryFactory().CreatePolygon(vertices.ToClosedCoordinates());

                    return (IFeature)poly.ToFeature(name);
                }).ToList();

                dict.Add(item.Key, list);
            }

            return dict;
        }
    }
}
