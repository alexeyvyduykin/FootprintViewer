using FootprintViewer.Data;
using FootprintViewer.Data.Science;
using FootprintViewer.ViewModels;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public interface ISensorLayerSource : ILayer
    {
        void Update(SatelliteInfo info);
    }

    public class SensorLayerSource : WritableLayer, ISensorLayerSource
    {
        private readonly Dictionary<string, Dictionary<int, List<GeometryFeature>>> _dictLeft;
        private readonly Dictionary<string, Dictionary<int, List<GeometryFeature>>> _dictright;
        private readonly Dictionary<string, List<GeometryFeature>> _cache;
        private readonly SatelliteProvider _provider;

        public SensorLayerSource(SatelliteProvider provider)
        {
            _provider = provider;

            _cache = new Dictionary<string, List<GeometryFeature>>();

            _dictLeft = new Dictionary<string, Dictionary<int, List<GeometryFeature>>>();

            _dictright = new Dictionary<string, Dictionary<int, List<GeometryFeature>>>();

            provider.Loading.Subscribe(LoadingImpl);
        }

        private void LoadingImpl(List<Satellite> satellites)
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
                    _cache.Add(name, new List<GeometryFeature>());
                }
            }
        }

        public void Update(SatelliteInfo info)
        {
            var name = info.Name;

            if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
            {
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

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
            }
        }

        private Dictionary<int, List<GeometryFeature>> FromStrips(string name, Dictionary<int, List<List<(double lon, double lat)>>> strips)
        {
            var dict = new Dictionary<int, List<GeometryFeature>>();
            foreach (var item in strips)
            {
                var list = new List<GeometryFeature>();

                foreach (var ln in item.Value)
                {
                    //var ring = new LinearRing();
                    var vertices = new List<Coordinate>();

                    foreach (var p in ln)
                    {
                        var point = SphericalMercator.FromLonLat(p.lon * ScienceMath.RadiansToDegrees, p.lat * ScienceMath.RadiansToDegrees).ToCoordinate();
                        vertices.Add(point);
                    }

                    //var poly = new Polygon() { ExteriorRing = ring };

                    var poly = new GeometryFactory().CreatePolygon(vertices.ToArray().ToClosedCoordinates());

                    list.Add(new GeometryFeature
                    {
                        Geometry = poly,
                        ["Name"] = name,
                    });
                }

                dict.Add(item.Key, list);
            }

            return dict;
        }
    }
}
