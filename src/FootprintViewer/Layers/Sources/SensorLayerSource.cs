﻿using FootprintViewer.Data;
using FootprintViewer.Data.Science;
using FootprintViewer.ViewModels;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
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
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dictLeft;
        private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _dictright;
        private readonly Dictionary<string, List<IFeature>> _cache;
        private readonly SatelliteProvider _provider;

        public SensorLayerSource(SatelliteProvider provider)
        {
            _provider = provider;

            _cache = new Dictionary<string, List<IFeature>>();

            _dictLeft = new Dictionary<string, Dictionary<int, List<IFeature>>>();

            _dictright = new Dictionary<string, Dictionary<int, List<IFeature>>>();

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
                    _cache.Add(name, new List<IFeature>());
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

        private Dictionary<int, List<IFeature>> FromStrips(string name, Dictionary<int, List<List<NetTopologySuite.Geometries.Point>>> strips)
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

                    list.Add(new Feature
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