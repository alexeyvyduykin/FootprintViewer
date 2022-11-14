using BruTile.MbTiles;
using FootprintViewer.Data;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using NetTopologySuite.Geometries;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{
    public static class MapsuiExtensions
    {
        public static void SetWorldMapLayer(this Map map, MapResource resource)
        {
            var layer = CreateWorldMapLayer(resource);
            map.ReplaceLayer(layer, LayerType.WorldMap);
            map.Limiter = new ViewportLimiterKeepWithin { PanLimits = layer.Extent /*Envelope*/ };
        }

        public static void AddLayer(this IMap map, ILayer layer, LayerType layerType)
        {
            map.AddLayer(layer, layerType.ToString());
        }

        public static void AddLayer(this IMap map, ILayer layer, string name)
        {
            layer.Name = name;

            map.Layers.Add(layer);
        }

        public static void RemoveLayer(this IMap map, LayerType layerType)
        {
            map.RemoveLayer(layerType.ToString());
        }

        public static void RemoveLayer(this IMap map, string name)
        {
            var layer = map.Layers.FindLayer(name).FirstOrDefault();

            if (layer != null)
            {
                map.Layers.Remove(layer);
            }
        }

        public static T? GetLayer<T>(this IMap map, LayerType layerType) where T : ILayer
        {
            return (T?)map.Layers.FirstOrDefault(l => l.Name.Equals(layerType.ToString()));
        }

        public static ILayer? GetLayer(this IMap map, LayerType layerType)
        {
            return map.GetLayer<ILayer>(layerType);
        }

        public static void ReplaceLayer(this IMap map, ILayer layer, LayerType layerType)
        {
            map.ReplaceLayer(layer, layerType.ToString());
        }

        public static void ReplaceLayer(this IMap map, ILayer layer, string name)
        {
            int index = 0;
            ILayer? removable = null;

            var count = map.Layers.Count;

            for (int i = 0; i < count; i++)
            {
                if (map.Layers[i].Name.Equals(name) == true)
                {
                    removable = map.Layers[i];
                    index = i;
                    break;
                }
            }

            if (removable != null)
            {
                map.Layers.Remove(removable);

                layer.Name = name;

                map.Layers.Insert(index, layer);
            }
        }

        private static ILayer CreateWorldMapLayer(MapResource resource)
        {
            string path = resource.Path;

            var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

            //         var area = mbTilesTileSource.Schema.Extent.ToBoundingBox();
            //         var delta = area.Width / 2.0;

            //double minX, double minY, double maxX, double maxY
            //          var limiter = new BoundingBox(area.Left - delta, area.Bottom, area.Right + delta, area.Top);

            return new TileLayer(mbTilesTileSource);
        }

        public static IFeature? FindFeature(this WritableLayer layer, string name)
        {
            return layer?.GetFeatures().Where(f => string.Equals(name, (string?)f["Name"])).FirstOrDefault();
        }

        public static void ForceUpdate(this IMap map)
        {
            var temp = new Layer();
            map.Layers.Add(temp);
            map.Layers.Remove(new[] { temp });
        }
    }

    public static class GeometryIterator
    {
        public static IEnumerable<Point> AllVertices(this Geometry geometry)
        {
            if (geometry == null)
                return Array.Empty<Point>();

            var point = geometry as Point;
            if (point != null)
                return new[] { point };
            var lineString = geometry as LineString;
            if (lineString != null)
                return AllVertices(lineString);
            var polygon = geometry as Polygon;
            if (polygon != null)
                return AllVertices(polygon);
            if (geometry is IEnumerable<Geometry> geometries)
                return AllVertices(geometries);

            var format = $"unsupported geometry: {geometry.GetType().Name}";
            throw new NotSupportedException(format);
        }

        private static IEnumerable<Point> AllVertices(LineString lineString)
        {
            if (lineString == null)
                throw new ArgumentNullException(nameof(lineString));

            return lineString.Coordinates.Select(s => s.ToPoint());// Vertices;
        }

        private static IEnumerable<Point> AllVertices(Polygon polygon)
        {
            if (polygon == null)
                throw new ArgumentNullException(nameof(polygon));

            foreach (var point in polygon.ExteriorRing.Coordinates.Select(s => s.ToPoint())/*Vertices*/)
                yield return point;
            foreach (var ring in polygon.InteriorRings)
                foreach (var point in ring.Coordinates.Select(s => s.ToPoint())/*Vertices*/)
                    yield return point;
        }

        private static IEnumerable<Point> AllVertices(IEnumerable<Geometry> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var geometry in collection)
                foreach (var point in AllVertices(geometry))
                    yield return point;
        }
    }

    public static class TempExtensions
    {
        public static Coordinate[] ToClosedCoordinates(this Coordinate[] coordinates)
        {
            var first = coordinates[0];

            var list = coordinates.ToList();

            if (first != list.Last())
            {
                list.Add(first);
            }

            return list.ToArray();
        }

        public static Coordinate[] ToClosedCoordinates(this IEnumerable<(double, double)> values)
        {
            var coordinates = values.Select(s => s.ToCoordinate()).ToList();

            var first = coordinates.First();

            if (first != coordinates.Last())
            {
                coordinates.Add(first);
            }

            return coordinates.ToArray();
        }

        public static Coordinate[] ToGreaterThanTwoCoordinates(this IEnumerable<(double, double)> values)
        {
            var coordinates = values.Select(s => s.ToCoordinate()).ToArray();

            if (coordinates.Length >= 2)
            {
                return coordinates;
            }

            if (coordinates.Length == 0)
            {
                return new Coordinate[] { new Coordinate(0.0, 0.0), new Coordinate(0.0, 0.0) };
            }

            return new Coordinate[] { coordinates[0], coordinates[0] };
        }

        public static GeometryFeature ToFeature(this Geometry geometry, string name)
        {
            var feature = geometry.ToFeature();

            feature["Name"] = name;

            return feature;
        }
    }
}
