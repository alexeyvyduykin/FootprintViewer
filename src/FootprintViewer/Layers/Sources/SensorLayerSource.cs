﻿using FootprintViewer.Data;
using Mapsui;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public interface ISensorLayerSource : ILayerSource
    {
        void Update(string name, int node, bool isShowLeft, bool isShowRight);

        void UpdateData(List<Satellite> satellites);
    }

    public class SensorLayerSource : BaseLayerSource<Satellite>, ISensorLayerSource
    {
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dictLeft = new();
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dictright = new();
        private readonly Dictionary<string, List<IFeature>> _cache = new();

        public void UpdateData(List<Satellite> satellites)
        {
            var leftStrips = StripBuilder.CreateLeft(satellites);
            var rightStrips = StripBuilder.CreateRight(satellites);

            _dictLeft.Clear();
            _dictright.Clear();

            _cache.Clear();

            foreach (var sat in satellites)
            {
                var name = sat.Name!;
                var dictLeft = FeatureBuilder.Build(name, leftStrips[name]);
                var dictRight = FeatureBuilder.Build(name, rightStrips[name]);

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
    }
}
