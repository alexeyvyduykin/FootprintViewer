using FootprintViewer.Data;
using Mapsui;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public interface IGroundStationLayerSource : ILayerSource
    {
        void UpdateData(List<GroundStation> groundStations);

        void Update(string name, Point center, double[] angles, bool isShow);

        void Change(string name, Point center, double[] angles, bool isShow);
    }

    public class GroundStationLayerSource : BaseLayerSource<GroundStation>, IGroundStationLayerSource
    {
        private readonly Dictionary<string, List<IFeature>> _cache = new();

        public void UpdateData(List<GroundStation> groundStations)
        {
            foreach (var item in groundStations)
            {
                if (string.IsNullOrEmpty(item.Name) == false)
                {
                    _cache.Add(item.Name, new List<IFeature>());
                }
            }
        }

        public void Update(string name, Point center, double[] angles, bool isShow)
        {
            if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
            {
                _cache[name].Clear();

                if (isShow == true)
                {
                    var groundStation = new GroundStation()
                    {
                        Name = name,
                        Center = center,
                        Angles = angles,
                    };

                    _cache[name] = FeatureBuilder.Build(groundStation);
                }

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
                DataHasChanged();
            }
        }

        public void Change(string name, Point center, double[] angles, bool isShow)
        {
            if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
            {
                _cache[name].Clear();

                if (isShow == true)
                {
                    var groundStation = new GroundStation()
                    {
                        Name = name,
                        Center = center,
                        Angles = angles,
                    };

                    _cache[name] = FeatureBuilder.Build(groundStation);
                }

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
                DataHasChanged();
            }
        }
    }
}
