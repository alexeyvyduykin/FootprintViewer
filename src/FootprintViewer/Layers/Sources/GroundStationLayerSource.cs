using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public interface IGroundStationLayerSource : ILayerSource
    {
        void Update(GroundStationInfo info);

        void Change(GroundStationInfo info);
    }

    public class GroundStationLayerSource : BaseLayerSource<GroundStationInfo>, IGroundStationLayerSource
    {
        private readonly Dictionary<string, List<IFeature>> _cache;

        public GroundStationLayerSource(IProvider<GroundStationInfo> provider) : base(provider)
        {
            _cache = new Dictionary<string, List<IFeature>>();
        }

        protected override void LoadingImpl(List<GroundStationInfo> groundStations)
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
                        Name = name,
                        Center = new Point(info.Center),
                        Angles = info.GetAngles(),
                    };

                    _cache[name] = Build(groundStation);
                }

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
                DataHasChanged();
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
                        Center = new Point(info.Center),
                        Angles = info.GetAngles(),
                    };

                    _cache[name] = Build(groundStation);
                }

                Clear();
                AddRange(_cache.SelectMany(s => s.Value));
                DataHasChanged();
            }
        }

        private static List<IFeature> Build(GroundStation groundStation)
        {
            var list = new List<IFeature>();

            var gs = GroundStationBuilder.Create(groundStation);

            var areaCount = gs.Areas.Count;

            bool isHole = (gs.InnerAngle != 0.0);

            // First area
            if (isHole == false)
            {
                var poligons = gs.Areas.First()
                    .Select(s => s.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat)))
                    .Select(s => new GeometryFactory().CreatePolygon(s.ToClosedCoordinates()))
                    .ToArray();

                var multi = new GeometryFactory().CreateMultiPolygon(poligons);

                var feature = multi.ToFeature();

                feature["Count"] = $"{areaCount}";
                feature["Index"] = $"{0}";

                list.Add(feature);
            }

            // Areas
            if (areaCount > 1)
            {
                for (int i = 1; i < areaCount; i++)
                {
                    var poligons = new List<Polygon>();

                    var rings = gs.Areas[i - 1].Select(s => s.Reverse().Select(s => SphericalMercator.FromLonLat(s.lon, s.lat))).ToList();

                    int index = 0;

                    foreach (var points1 in gs.Areas[i])
                    {
                        var res = points1.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat));

                        if (index < rings.Count)
                        {
                            var interiorRings = (gs.Areas[i].Count() == 1) ? rings : new List<IEnumerable<(double, double)>>() { rings[index++] };

                            var shell = new GeometryFactory().CreateLinearRing(res.ToClosedCoordinates());
                            var holes = interiorRings.Select(s => new GeometryFactory().CreateLinearRing(s.ToClosedCoordinates())).ToArray();

                            var poly = new GeometryFactory().CreatePolygon(shell, holes);

                            poligons.Add(poly);
                        }
                        else
                        {
                            var poly = new GeometryFactory().CreatePolygon(res.ToClosedCoordinates());

                            poligons.Add(poly);
                        }
                    }

                    var multi = new GeometryFactory().CreateMultiPolygon(poligons.ToArray());

                    var feature = multi.ToFeature();

                    feature["Count"] = $"{(isHole == true ? areaCount - 1 : areaCount)}";
                    feature["Index"] = $"{(isHole == true ? i - 1 : i)}";

                    list.Add(feature);
                }
            }

            // Inner border
            if (isHole == true)
            {
                var lineStrings = gs.InnerBorder
                    .Select(s => s.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate()).ToArray())
                    .Select(s => new GeometryFactory().CreateLineString(s))
                    .ToArray();

                var multi = new GeometryFactory().CreateMultiLineString(lineStrings);

                var feature = multi.ToFeature();

                feature["Count"] = $"{(isHole == true ? areaCount - 1 : areaCount)}";
                feature["InnerBorder"] = "";

                list.Add(feature);
            }

            // Outer border
            if (gs.OuterBorder.Any())
            {
                var lineStrings = gs.OuterBorder
                    .Select(s => s.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate()).ToArray())
                    .Select(s => new GeometryFactory().CreateLineString(s))
                    .ToArray();

                var multi = new GeometryFactory().CreateMultiLineString(lineStrings);

                var feature = multi.ToFeature();

                feature["Count"] = $"{(isHole == true ? areaCount - 1 : areaCount)}";
                feature["OuterBorder"] = "";

                list.Add(feature);
            }

            return list;
        }
    }
}
