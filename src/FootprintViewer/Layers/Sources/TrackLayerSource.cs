using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public interface ITrackLayerSource : ILayer
    {
        void Update(SatelliteInfo info);
    }

    public class TrackLayerSource : WritableLayer, ITrackLayerSource
    {
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dict;
        private readonly Dictionary<string, List<IFeature>> _cache;

        public TrackLayerSource(SatelliteProvider provider)
        {
            _dict = new Dictionary<string, Dictionary<int, List<IFeature>>>();

            _cache = new Dictionary<string, List<IFeature>>();

            provider.Loading.Subscribe(LoadingImpl);
        }

        private void LoadingImpl(List<SatelliteInfo> satellites)
        {
            var tracks = TrackBuilder.Create(satellites.Select(s => s.Satellite));

            _dict.Clear();

            _cache.Clear();

            foreach (var sat in satellites)
            {
                var name = sat.Name!;
                var dict = new Dictionary<int, List<IFeature>>();

                foreach (var item in tracks[name])
                {
                    var list = item.Value.Select(s =>
                    {
                        var vertices = s.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat));

                        var line = new GeometryFactory().CreateLineString(vertices.ToGreaterThanTwoCoordinates());

                        return (IFeature)line.ToFeature(name);
                    }).ToList();

                    dict.Add(item.Key, list);
                }

                _dict.Add(name, dict);
                _cache.Add(name, new List<IFeature>());
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
                DataHasChanged();
            }
        }
    }
}
