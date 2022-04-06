using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public interface IGroundStationLayerSource : ILayer
    {
        void Update(GroundStationInfo info);
    }

    public class GroundStationLayerSource : WritableLayer, IGroundStationLayerSource
    {
        private readonly Dictionary<string, List<IFeature>> _cache;

        public GroundStationLayerSource(GroundStationProvider provider)
        {
            //Update = ReactiveCommand.Create<GroundStation>(UpdateImpl);

            _cache = new Dictionary<string, List<IFeature>>();

            Loading = ReactiveCommand.Create<List<GroundStation>>(LoadingImpl);

            provider.Loading.Select(s => s).InvokeCommand(Loading);
        }

        //private ReactiveCommand<GroundStation, Unit> Update { get; }

        private ReactiveCommand<List<GroundStation>, Unit> Loading { get; }

        private void LoadingImpl(List<GroundStation> groundStations)
        {
            foreach (var item in groundStations)
            {
                if (string.IsNullOrEmpty(item.Name) == false)
                {
                    _cache.Add(item.Name, new List<IFeature>());
                }
            }
        }

        public void Update(GroundStationInfo info)
        {
            var name = info.Name;

            if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
            {
                _cache[name].Clear();

                if (info.IsShow == true)
                {
                    var groundStation = new GroundStation()
                    {
                        Center = new NetTopologySuite.Geometries.Point(info.Center),
                        Angles = info.Angles,
                    };

                    _cache[name].Add(Build(groundStation));
                }

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
            }
        }

        private static List<IFeature> Build(GroundStation groundStation)
        {
            var list = new List<IFeature>();

            var gs = GroundStationBuilder.Create(groundStation);

            var areaCount = gs.Areas.Count;

            // First area
            if (gs.InnerAngle == 0.0)
            {
                var multi = new MultiPolygon();

                foreach (var points in gs.Areas.First())
                {
                    var ring = new LinearRing(points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));

                    multi.Polygons.Add(new Polygon()
                    {
                        ExteriorRing = ring,
                    });
                }

                var feature = new Feature
                {
                    Geometry = multi,
                    ["Count"] = $"{areaCount}",
                    ["Index"] = $"{0}",
                };

                list.Add(feature);
            }

            // Areas
            if (areaCount > 1)
            {
                for (int i = 1; i < areaCount; i++)
                {
                    var multi = new MultiPolygon();

                    var rings = gs.Areas[i - 1].Select(s =>
                      new LinearRing(s.Reverse().Select(s => SphericalMercator.FromLonLat(s.X, s.Y)))).ToList();

                    int index = 0;

                    foreach (var points1 in gs.Areas[i])
                    {
                        var ring = new LinearRing(points1.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));

                        if (index < rings.Count)
                        {
                            var interiorRings = (gs.Areas[i].Count() == 1) ? rings : new List<LinearRing>() { rings[index++] };

                            multi.Polygons.Add(new Polygon()
                            {
                                ExteriorRing = ring,
                                InteriorRings = interiorRings,
                            });
                        }
                        else
                        {
                            multi.Polygons.Add(new Polygon()
                            {
                                ExteriorRing = ring,
                            });
                        }
                    }

                    var feature = new Feature
                    {
                        Geometry = multi,
                        ["Count"] = $"{areaCount}",
                        ["Index"] = $"{i}",
                    };

                    list.Add(feature);
                }
            }

            // Inner border
            if (gs.InnerAngle != 0.0)
            {
                var multi = new MultiLineString
                {
                    LineStrings = gs.InnerBorder.Select(s =>
                      new LineString(s.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)))).ToList()
                };

                var feature = new Feature
                {
                    Geometry = multi,
                    ["Count"] = $"{areaCount}",
                    ["InnerBorder"] = "",
                };

                list.Add(feature);
            }

            // Outer border
            if (gs.OuterBorder.Any())
            {
                var multi = new MultiLineString
                {
                    LineStrings = gs.OuterBorder.Select(s =>
                      new LineString(s.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)))).ToList()
                };

                var feature = new Feature
                {
                    Geometry = multi,
                    ["Count"] = $"{areaCount}",
                    ["OuterBorder"] = "",
                };

                list.Add(feature);
            }

            return list;
        }
    }
}
