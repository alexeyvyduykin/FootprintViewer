using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui.Nts.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace FootprintViewer.Layers
{
    public interface ITrackLayerSource : ILayer
    {
        void Update(SatelliteInfo info);
    }

    public class TrackLayerSource : WritableLayer, ITrackLayerSource
    {
        private readonly Dictionary<string, Dictionary<int, List<GeometryFeature>>> _dict;
        private readonly Dictionary<string, List<GeometryFeature>> _cache;
        private readonly SatelliteProvider _provider;

        public TrackLayerSource(SatelliteProvider provider)
        {
            _provider = provider;

            _dict = new Dictionary<string, Dictionary<int, List<GeometryFeature>>>();

            _cache = new Dictionary<string, List<GeometryFeature>>();

            provider.Loading.Subscribe(LoadingImpl);
        }

        private void LoadingImpl(List<Satellite> satellites)
        {
            var tracks = TrackBuilder.Create(satellites);

            _dict.Clear();

            _cache.Clear();

            foreach (var sat in satellites)
            {
                var name = sat.Name!;
                var dict = new Dictionary<int, List<GeometryFeature>>();

                foreach (var item in tracks[name])
                {
                    var list = new List<GeometryFeature>();

                    foreach (var ln in item.Value)
                    {
                        //var line = new LineString();

                        var vertices = new List<Coordinate>();

                        foreach (var (lon, lat) in ln)
                        {
                            var point = SphericalMercator.FromLonLat(lon, lat).ToCoordinate();
                            vertices.Add(point);
                        }

                        var line = new GeometryFactory().CreateLineString(vertices.ToArray());
                      
                        list.Add(new GeometryFeature
                        {
                            Geometry = line,
                            ["Name"] = name
                        });
                    }

                    dict.Add(item.Key, list);
                }

                _dict.Add(name, dict);
                _cache.Add(name, new List<GeometryFeature>());
            }
        }

        public void Update(SatelliteInfo info)
        {
            var name = info.Name;

            if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
            {
                _cache[name].Clear();

                if (info.IsShow == true && info.IsTrack == true)
                {
                    var node = info.CurrentNode;

                    if (_dict.ContainsKey(name) == true && _dict[name].ContainsKey(node) == true)
                    {
                        var features = _dict[name][node];
                        _cache[name].AddRange(features);
                    }
                }

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
            }
        }
    }
}
