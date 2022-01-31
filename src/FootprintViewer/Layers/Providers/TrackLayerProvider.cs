using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Providers;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public class TrackLayerProvider : MemoryProvider
    {
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dict = new Dictionary<string, Dictionary<int, List<IFeature>>>();
        private readonly Dictionary<string, List<IFeature>> _cache = new Dictionary<string, List<IFeature>>();

        public TrackLayerProvider(IDataSource source)
        {
            var satellites = source.GroundTracks.Keys;

            foreach (var name in satellites)
            {        
                var dict = new Dictionary<int, List<IFeature>>();

                foreach (var item in source.GroundTracks[name])
                {
                    var list = new List<IFeature>();

                    foreach (var ln in item.Value)
                    {
                        var line = new LineString();

                        foreach (var (lon, lat) in ln)
                        {
                            var point = SphericalMercator.FromLonLat(lon, lat);
                            line.Vertices.Add(point);
                        }

                        list.Add(new Feature { Geometry = line });
                    }

                    dict.Add(item.Key, list);
                }

                _dict.Add(name, dict);
                _cache.Add(name, new List<IFeature>());
            }
        }

        public void Update(SatelliteInfo info)
        {
            var name = info.Name;

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

            ReplaceFeatures(_cache.SelectMany(s => s.Value));

            
        }
    }
}
