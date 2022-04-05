using FootprintViewer.Data;
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

    }

    public class GroundStationLayerSource : WritableLayer, IGroundStationLayerSource
    {
        private List<IFeature> _featuresCache = new();

        public GroundStationLayerSource(GroundStationProvider provider)
        {
            Update = ReactiveCommand.Create<GroundStation>(UpdateImpl);

            Loading = ReactiveCommand.Create<List<GroundStation>>(LoadingImpl);

            provider.Loading.Select(s => s).InvokeCommand(Loading);
        }

        private ReactiveCommand<GroundStation, Unit> Update { get; }

        private ReactiveCommand<List<GroundStation>, Unit> Loading { get; }

        private void LoadingImpl(List<GroundStation> groundStations)
        {
            _featuresCache = Build(groundStations);

            Clear();
            AddRange(_featuresCache);
        }

        private void UpdateImpl(GroundStation groundStation)
        {
            //_featuresCache = Build(groundStation);

            Clear();
            AddRange(_featuresCache);
        }

        private static List<IFeature> Build(List<GroundStation> gss)
        {
            var list = new List<IFeature>();

            var resGss = GroundStationBuilder.Create(gss);

            foreach (var gs in resGss)
            {
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
            }

            return list;
        }

        //private static List<IFeature> Build2(GroundStation groundStation)
        //{
        //    var list = new List<IFeature>();

        //    var res = EarthGeometry.BuildCircles(groundStation.Center.X, groundStation.Center.Y, groundStation.Angles);

        //    var areaCount = groundStation.Areas.Count;

        //    // First area
        //    if (groundStation.InnerAngle == 0.0)
        //    {
        //        var multi = new MultiPolygon();

        //        foreach (var points in groundStation.Areas.First())
        //        {
        //            var ring = new LinearRing(points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));

        //            multi.Polygons.Add(new Polygon()
        //            {
        //                ExteriorRing = ring,
        //            });
        //        }

        //        var feature = new Feature
        //        {
        //            Geometry = multi,
        //            ["Count"] = $"{areaCount}",
        //            ["Index"] = $"{0}",
        //        };

        //        list.Add(feature);
        //    }

        //    // Areas
        //    if (areaCount > 1)
        //    {
        //        for (int i = 1; i < areaCount; i++)
        //        {
        //            var multi = new MultiPolygon();

        //            var rings = groundStation.Areas[i - 1].Select(s =>
        //              new LinearRing(s.Reverse().Select(s => SphericalMercator.FromLonLat(s.X, s.Y)))).ToList();

        //            int index = 0;

        //            foreach (var points1 in groundStation.Areas[i])
        //            {
        //                var ring = new LinearRing(points1.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));

        //                if (index < rings.Count)
        //                {
        //                    var interiorRings = (groundStation.Areas[i].Count() == 1) ? rings : new List<LinearRing>() { rings[index++] };

        //                    multi.Polygons.Add(new Polygon()
        //                    {
        //                        ExteriorRing = ring,
        //                        InteriorRings = interiorRings,
        //                    });
        //                }
        //                else
        //                {
        //                    multi.Polygons.Add(new Polygon()
        //                    {
        //                        ExteriorRing = ring,
        //                    });
        //                }
        //            }

        //            var feature = new Feature
        //            {
        //                Geometry = multi,
        //                ["Count"] = $"{areaCount}",
        //                ["Index"] = $"{i}",
        //            };

        //            list.Add(feature);
        //        }
        //    }

        //    // Inner border
        //    if (groundStation.InnerAngle != 0.0)
        //    {
        //        var multi = new MultiLineString
        //        {
        //            LineStrings = groundStation.InnerBorder.Select(s =>
        //              new LineString(s.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)))).ToList()
        //        };

        //        var feature = new Feature
        //        {
        //            Geometry = multi,
        //            ["Count"] = $"{areaCount}",
        //            ["InnerBorder"] = "",
        //        };

        //        list.Add(feature);
        //    }

        //    // Outer border
        //    if (groundStation.OuterBorder.Any())
        //    {
        //        var multi = new MultiLineString
        //        {
        //            LineStrings = groundStation.OuterBorder.Select(s =>
        //              new LineString(s.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)))).ToList()
        //        };

        //        var feature = new Feature
        //        {
        //            Geometry = multi,
        //            ["Count"] = $"{areaCount}",
        //            ["OuterBorder"] = "",
        //        };

        //        list.Add(feature);
        //    }

        //    return list;
        //}
    }
}
