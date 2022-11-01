using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Layers;

public class GroundStationLayer : WritableLayer
{
    private readonly Dictionary<string, List<IFeature>> _cache = new();

    public void UpdateData(List<GroundStation> groundStations)
    {
        _cache.Clear();

        foreach (var item in groundStations)
        {
            if (string.IsNullOrEmpty(item.Name) == false)
            {
                _cache.Add(item.Name, new List<IFeature>());
            }
        }

        // TODO: temp solution
        Clear();
        DataHasChanged();
    }

    public void Update(GroundStationViewModel groundStation)
    {
        var name = groundStation.Name;
        var center = new NetTopologySuite.Geometries.Point(groundStation.Center);
        var angles = groundStation.GetAngles();
        var isShow = groundStation.IsShow;

        if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
        {
            _cache[name].Clear();

            if (isShow == true)
            {
                var gs = new GroundStation()
                {
                    Name = name,
                    Center = center,
                    Angles = angles,
                };

                _cache[name] = FeatureBuilder.Build(gs);
            }

            Clear();
            AddRange(_cache.SelectMany(s => s.Value));
            DataHasChanged();
        }
    }
}
