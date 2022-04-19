using FootprintViewer.Data;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Overlay;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public interface ITargetLayerSource : ILayer
    {
        ReactiveCommand<Unit, string[]?> Refresh { get; }

        IEnumerable<IFeature>? ActiveFeatures { get; set; }

        void SelectGroundTarget(string name);

        void ShowHighlight(string name);

        void HideHighlight();
    }

    public class TargetLayerSource : WritableLayer, ITargetLayerSource
    {
        private List<IFeature> _featuresCache = new List<IFeature>();
        private IFeature? _lastSelected;

        public TargetLayerSource(GroundTargetProvider provider)
        {
            provider.Loading.Subscribe(LoadingImpl);

            Refresh = ReactiveCommand.Create(RefreshImpl);
        }

        public ReactiveCommand<Unit, string[]?> Refresh { get; }

        public IEnumerable<IFeature>? ActiveFeatures { get; set; }

        private string[]? RefreshImpl()
        {
            if (ActiveFeatures == null)
            {
                return null;
            }

            return ActiveFeatures.Where(s => s.Fields.Contains("Name")).Select(s => (string)s["Name"]).ToArray();
        }

        private void LoadingImpl(List<GroundTarget> groundTargets)
        {
            _featuresCache = Build(groundTargets);

            Clear();
            AddRange(_featuresCache);
        }

        public void SelectGroundTarget(string name)
        {
            var feature = _featuresCache.Where(s => name.Equals((string)s["Name"])).First();

            if (_lastSelected != null)
            {
                _lastSelected["State"] = "Unselected";
            }

            feature["State"] = "Selected";

            _lastSelected = feature;

            DataHasChanged();
        }

        public void ShowHighlight(string name)
        {
            var feature = _featuresCache.Where(s => name.Equals((string)s["Name"])).First();

            feature["Highlight"] = true;

            DataHasChanged();
        }

        public void HideHighlight()
        {
            _featuresCache.ForEach(s => s["Highlight"] = false);

            DataHasChanged();
        }

        private List<IFeature> Build(IEnumerable<GroundTarget> groundTargets)
        {
            List<IFeature> list = new List<IFeature>();

            foreach (var item in groundTargets)
            {
                var feature = new GeometryFeature()
                {
                    ["Name"] = item.Name,
                    ["State"] = "Unselected",
                    ["Highlight"] = false,
                };

                switch (item.Type)
                {
                    case GroundTargetType.Point:
                    {
                        var p = (NetTopologySuite.Geometries.Point)item.Points!;
                        var point = SphericalMercator.FromLonLat(p.X, p.Y).ToMPoint().ToPoint();
                        feature.Geometry = point;
                        feature["Type"] = "Point";
                    }
                    break;
                    case GroundTargetType.Route:
                    {
                        feature.Geometry = RouteCutting(item.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList());
                        feature["Type"] = "Route";
                    }
                    break;
                    case GroundTargetType.Area:
                    {
                        feature.Geometry = AreaCutting(item.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList());
                        feature["Type"] = "Area";
                    }
                    break;
                    default:
                        throw new Exception();
                }

                list.Add(feature);
            }

            return list;
        }

        private Geometry AreaCutting(IList<Point> points)
        {
            var count = points.Count;
            //var ring1 = new LinearRing();
            //var ring2 = new LinearRing();

            //var ring = ring1;

            var vertices1 = new List<MPoint>();
            var vertices2 = new List<MPoint>();
            var vertices = vertices1;

            for (int i = 0; i < count; i++)
            {
                var p1 = points[i];
                var p2 = (i == count - 1) ? points[0] : points[i + 1];

                var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y).ToMPoint();
                vertices.Add(point1);

                if (Math.Abs(p2.X - p1.X) > 180)
                {
                    if (p2.X - p1.X > 0) // -180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(-180, cutLat).ToMPoint();
                        vertices.Add(pp1);

                        vertices = (vertices == vertices1) ? vertices2 : vertices1;

                        var pp2 = SphericalMercator.FromLonLat(180, cutLat).ToMPoint();
                        vertices.Add(pp2);
                    }

                    if (p2.X - p1.X < 0) // +180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(180, cutLat).ToMPoint();
                        vertices.Add(pp1);

                        vertices = (vertices == vertices1) ? vertices2 : vertices1;

                        var pp2 = SphericalMercator.FromLonLat(-180, cutLat).ToMPoint();
                        vertices.Add(pp2);
                    }
                }


            }


            if (vertices2.Count != 0) // multipolygon
            {
                //var ring1 = new LinearRing();
                //var ring2 = new LinearRing();
        
                //multi.Polygons.Add(new Polygon() { ExteriorRing = ring1 });
                //multi.Polygons.Add(new Polygon() { ExteriorRing = ring2 });

                var poly1 = new GeometryFactory().CreatePolygon(vertices1.Select(s=>s.ToCoordinate()).ToArray().ToClosedCoordinates());
                var poly2 = new GeometryFactory().CreatePolygon(vertices2.Select(s => s.ToCoordinate()).ToArray().ToClosedCoordinates());


                var multi = new GeometryFactory().CreateMultiPolygon(new[] { poly1, poly2 });

                return multi;
            }
            else
            {
                var poly1 = new GeometryFactory().CreatePolygon(vertices1.Select(s => s.ToCoordinate()).ToArray().ToClosedCoordinates());
                return poly1;

                //return new Polygon() { ExteriorRing = ring1 };
            }
        }

        private Geometry RouteCutting(IList<Point> points)
        {
            var count = points.Count;
            //var line1 = new LineString();
            //var line2 = new LineString();

            //var line = line1;

            var vertices1 = new List<MPoint>();
            var vertices2 = new List<MPoint>();
            var vertices = vertices1;

            for (int i = 0; i < count - 1; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];

                var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y).ToMPoint();
                vertices.Add(point1);

                if (Math.Abs(p2.X - p1.X) > 180)
                {
                    if (p2.X - p1.X > 0) // -180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(-180, cutLat).ToMPoint();
                        vertices.Add(pp1);

                        vertices = (vertices == vertices1) ? vertices2 : vertices1;

                        var pp2 = SphericalMercator.FromLonLat(180, cutLat).ToMPoint();
                        vertices.Add(pp2);
                    }

                    if (p2.X - p1.X < 0) // +180 cutting
                    {
                        var cutLat = LinearInterpDiscontLat(p1, p2);
                        var pp1 = SphericalMercator.FromLonLat(180, cutLat).ToMPoint();
                        vertices.Add(pp1);

                        vertices = (vertices == vertices1) ? vertices2 : vertices1;

                        var pp2 = SphericalMercator.FromLonLat(-180, cutLat).ToMPoint();
                        vertices.Add(pp2);
                    }
                }


            }


            if (vertices2.Count != 0) // MultiLineString
            {
                //var multi = new MultiLineString();
                // multi.LineStrings.Add(line1);
                // multi.LineStrings.Add(line2);
                
                var line1 = new LineString(vertices1.Select(s=>s.ToCoordinate()).ToArray());
                var line2 = new LineString(vertices2.Select(s => s.ToCoordinate()).ToArray().ToGreaterThanTwoCoordinates());

                var multi = new GeometryFactory().CreateMultiLineString(new[] { line1 , line2 });

                return multi;
            }
            else
            {
                var line1 = new LineString(vertices1.Select(s => s.ToCoordinate()).ToArray());
                return line1;
            }
        }

        private double LinearInterpDiscontLat(Point p1, Point p2)
        {
            // one longitude should be negative one positive, make them both positive
            double lon1 = p1.X, lat1 = p1.Y, lon2 = p2.X, lat2 = p2.Y;
            if (lon1 > lon2)
            {
                lon2 += 360;
            }
            else
            {
                lon1 += 360;
            }

            return (lat1 + (180 - lon1) * (lat2 - lat1) / (lon2 - lon1));
        }
    }
}
