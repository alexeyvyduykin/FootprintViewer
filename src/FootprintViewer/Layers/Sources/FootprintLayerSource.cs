﻿using FootprintViewer.Data;
using Mapsui.Extensions;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;

namespace FootprintViewer.Layers
{
    public interface IFootprintLayerSource : ILayer
    {
        void SelectFeature(string name);

        void UnselectFeature(string name);
    }

    public class FootprintLayerSource : WritableLayer, IFootprintLayerSource
    {
        private IFeature? _lastSelected;
        private List<IFeature> _featuresCache = new List<IFeature>();
        private readonly FootprintProvider _provider;

        public FootprintLayerSource(FootprintProvider provider)
        {
            _provider = provider;

            Loading = ReactiveCommand.Create<List<Footprint>>(LoadingImpl);

            provider.Loading.Select(s => s).InvokeCommand(Loading);
        }

        private ReactiveCommand<List<Footprint>, Unit> Loading { get; }

        private void LoadingImpl(List<Footprint> footprints)
        {
            _featuresCache = Build(footprints);
            Clear();
            AddRange(_featuresCache);
        }

        public Footprint GetFootprint(string name) => _provider.GetFootprints().Where(s => s.Name!.Equals(name)).FirstOrDefault()!;

        public async Task<List<Footprint>> GetFootprintsAsync() => await Task.Run(() => _provider.GetFootprints().ToList());

        public List<Footprint> GetFootprints() => _provider.GetFootprints().ToList();

        public void SelectFeature(string name)
        {
            var feature = _featuresCache.Where(s => name.Equals((string)s["Name"])).First();

            if (_lastSelected != null)
            {
                _lastSelected["State"] = "Unselect";
            }

            feature["State"] = "Select";

            _lastSelected = feature;

            DataHasChanged();
        }

        public void UnselectFeature(string name)
        {
            if (_lastSelected != null && name.Equals(_lastSelected["Name"]) == true)
            {
                _lastSelected["State"] = "Unselect";

                DataHasChanged();
            }
        }

        public bool IsSelect(string name)
        {
            var feature = _featuresCache.Where(s => name.Equals((string)s["Name"])).First();

            if (feature != null)
            {
                return (string)feature["State"] == "Select";
            }

            throw new Exception();
        }

        private List<IFeature> Build(IEnumerable<Footprint> footprints)
        {
            var list = new List<IFeature>();

            foreach (var item in footprints)
            {
                var poly = AreaCutting(item.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList());

                var feature = new GeometryFeature { Geometry = poly };

                feature["Name"] = item.Name;
                feature["State"] = "Unselect";

                list.Add(feature);
            }

            return list;
        }

        //private IGeometry AreaCutting(IList<Point> points)
        //{
        //    var count = points.Count;
        //    var ring1 = new LinearRing();
        //    var ring2 = new LinearRing();

        //    var ring = ring1;

        //    for (int i = 0; i < count; i++)
        //    {
        //        var p1 = points[i];
        //        var p2 = (i == count - 1) ? points[0] : points[i + 1];

        //        var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y);
        //        ring.Vertices.Add(point1);

        //        if (Math.Abs(p2.X - p1.X) > 180)
        //        {
        //            if (p2.X - p1.X > 0) // -180 cutting
        //            {
        //                var cutLat = LinearInterpDiscontLat(p1, p2);
        //                var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
        //                ring.Vertices.Add(pp1);

        //                ring = (ring == ring1) ? ring2 : ring1;

        //                var pp2 = SphericalMercator.FromLonLat(180, cutLat);
        //                ring.Vertices.Add(pp2);
        //            }

        //            if (p2.X - p1.X < 0) // +180 cutting
        //            {
        //                var cutLat = LinearInterpDiscontLat(p1, p2);
        //                var pp1 = SphericalMercator.FromLonLat(180, cutLat);
        //                ring.Vertices.Add(pp1);

        //                ring = (ring == ring1) ? ring2 : ring1;

        //                var pp2 = SphericalMercator.FromLonLat(-180, cutLat);
        //                ring.Vertices.Add(pp2);
        //            }
        //        }


        //    }


        //    if (ring2.Length != 0) // multipolygon
        //    {
        //        var multi = new MultiPolygon();
        //        multi.Polygons.Add(new Polygon() { ExteriorRing = ring1 });
        //        multi.Polygons.Add(new Polygon() { ExteriorRing = ring2 });

        //        return multi;
        //    }
        //    else
        //    {
        //        return new Polygon() { ExteriorRing = ring1 };
        //    }
        //}

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

                var poly1 = new GeometryFactory().CreatePolygon(vertices1.Select(s => s.ToCoordinate()).ToArray().ToClosedCoordinates());
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
