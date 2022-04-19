using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
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
        void Change(GroundStationInfo info);
    }

    public class GroundStationLayerSource : WritableLayer, IGroundStationLayerSource
    {
        private readonly Dictionary<string, List<GeometryFeature>> _cache;

        public GroundStationLayerSource(GroundStationProvider provider)
        {
            //Update = ReactiveCommand.Create<GroundStation>(UpdateImpl);

            _cache = new Dictionary<string, List<GeometryFeature>>();

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
                    _cache.Add(item.Name, new List<GeometryFeature>());
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
                        Name = name,
                        Center = new NetTopologySuite.Geometries.Point(info.Center),
                        Angles = info.GetAngles(),
                    };

                    _cache[name].Add(Build(groundStation));
                }

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
            }
        }
        public void Change(GroundStationInfo info)
        {
            var name = info.Name;

            if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
            {
                _cache[name].Clear();

                if (info.IsShow == true)
                {
                    var groundStation = new GroundStation()
                    {
                        Name = info.Name,
                        Center = new NetTopologySuite.Geometries.Point(info.Center),
                        Angles = info.GetAngles(),
                    };

                    _cache[name].Add(Build(groundStation));
                }

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
            }
        }
        private static List<GeometryFeature> Build(GroundStation groundStation)
        {
            var list = new List<GeometryFeature>();

            var gs = GroundStationBuilder.Create(groundStation);

            var areaCount = gs.Areas.Count;

            bool isHole = (gs.InnerAngle != 0.0);

            // First area
            if (isHole == false)
            {
                //var multi = new MultiPolygon();

                var poligons = new List<Polygon>();

                foreach (var points in gs.Areas.First())
                {
                    //var ring = new LinearRing(points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));

                    var res = points.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate());
                    var poly = new GeometryFactory().CreatePolygon(res.ToArray().ToClosedCoordinates());

                    //multi.Polygons.Add(new Polygon()
                    //{
                    //    ExteriorRing = ring,
                    //});

                    poligons.Add(poly);
                }


                var multi = new GeometryFactory().CreateMultiPolygon(poligons.ToArray());

                var feature = new GeometryFeature
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
                    //var multi = new MultiPolygon();

                    var poligons = new List<Polygon>();

              //      var rings = gs.Areas[i - 1].Select(s =>
              //        new LinearRing(s.Reverse().Select(s => SphericalMercator.FromLonLat(s.X, s.Y)))).ToList();

                    var rings = gs.Areas[i - 1].Select(s => s.Reverse().Select(s => SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate())).ToList();

                    int index = 0;

                    foreach (var points1 in gs.Areas[i])
                    {
                        //var ring = new LinearRing(points1.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));
                        var res = points1.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate());

                        if (index < rings.Count)
                        {
                           // var interiorRings = (gs.Areas[i].Count() == 1) ? rings : new List<LinearRing>() { rings[index++] };
                            var interiorRings = (gs.Areas[i].Count() == 1) ? rings : new List<IEnumerable<Coordinate>>() { rings[index++] };

                            //multi.Polygons.Add(new Polygon()
                            //{
                            //    ExteriorRing = ring,
                            //    InteriorRings = interiorRings,
                            //});

                            var shell = new GeometryFactory().CreateLinearRing(res.ToArray().ToClosedCoordinates());
                            var holes = interiorRings.Select(s => new GeometryFactory().CreateLinearRing(s.ToArray().ToClosedCoordinates()));

                            var poly = new GeometryFactory().CreatePolygon(shell, holes.ToArray());
                            poligons.Add(poly);
                        }
                        else
                        {
                            //multi.Polygons.Add(new Polygon()
                            //{
                            //    ExteriorRing = ring,
                            //});

                            var poly = new GeometryFactory().CreatePolygon(res.ToArray().ToClosedCoordinates());
                            poligons.Add(poly);
                        }
                    }

                    var multi = new GeometryFactory().CreateMultiPolygon(poligons.ToArray());


                    var feature = new GeometryFeature
                    {
                        Geometry = multi,
                        ["Count"] = $"{(isHole == true ? areaCount - 1 : areaCount)}",
                        ["Index"] = $"{(isHole == true ? i - 1 : i)}",
                    };

                    list.Add(feature);
                }
            }

            // Inner border
            if (isHole == true)
            {
                //var multi = new MultiLineString
                //{
                //    LineStrings = gs.InnerBorder.Select(s =>
                //      new LineString(s.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)))).ToList()
                //};

                var lineStrings = new List<LineString>();

                foreach (var item in gs.InnerBorder)
                {
                    var res = item.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate());

                    var line = new GeometryFactory().CreateLineString(res.ToArray());
                    lineStrings.Add(line);
                }

                var multi = new GeometryFactory().CreateMultiLineString(lineStrings.ToArray());

                var feature = new GeometryFeature
                {
                    Geometry = multi,
                    ["Count"] = $"{(isHole == true ? areaCount - 1 : areaCount)}",
                    ["InnerBorder"] = "",
                };

                list.Add(feature);
            }

            // Outer border
            if (gs.OuterBorder.Any())
            {
                var lineStrings = new List<LineString>();

                foreach (var item in gs.OuterBorder)
                {
                    var res = item.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate());

                    var line = new GeometryFactory().CreateLineString(res.ToArray());
                    lineStrings.Add(line);
                }

                //var multi = new MultiLineString
                //{
                //    LineStrings = gs.OuterBorder.Select(s =>
                //      new LineString(s.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)))).ToList()
                //};
                var multi = new GeometryFactory().CreateMultiLineString(lineStrings.ToArray());

                var feature = new GeometryFeature
                {
                    Geometry = multi,
                    ["Count"] = $"{(isHole == true ? areaCount - 1 : areaCount)}",
                    ["OuterBorder"] = "",
                };

                list.Add(feature);
            }

            return list;
        }
    }
}
