using FootprintViewer.Data;
using FootprintViewer.ViewModels.SidePanel.Items;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers;

public class TrackLayer : WritableLayer
{
    private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dict = new();
    private readonly Dictionary<string, List<IFeature>> _cache = new();

    public void UpdateData(IList<Satellite> satellites)
    {
        var tracks = TrackBuilder.Create(satellites);

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

        // TODO: temp solution
        Clear();
        DataHasChanged();
    }

    public void Update(SatelliteViewModel satellite)
    {
        var name = satellite.Name;
        var node = satellite.CurrentNode;
        var isShow = satellite.IsShow && satellite.IsTrack;

        if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
        {
            _cache[name].Clear();

            if (isShow == true)
            {
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
