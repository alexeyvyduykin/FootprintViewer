using BruTile.MbTiles;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Tiling.Layers;
using NetTopologySuite.Geometries;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer;

public static class MapsuiExtensions
{
    public static void SetWorldMapLayer(this Map map, MapResource resource)
    {
        var layer = CreateWorldMapLayer(resource);
        map.ReplaceLayer(layer, LayerType.WorldMap);
        //map.Limiter = new ViewportLimiterKeepWithin { PanLimits = layer.Extent /*Envelope*/ };
    }

    public static void AddLayer(this Map map, ILayer layer, LayerType layerType)
    {
        map.AddLayer(layer, layerType.ToString());
    }

    public static void AddLayer(this Map map, ILayer layer, string name)
    {
        layer.Name = name;

        map.Layers.Add(layer);
    }

    public static void RemoveLayer(this Map map, LayerType layerType)
    {
        map.RemoveLayer(layerType.ToString());
    }

    public static void RemoveLayer(this Map map, string name)
    {
        var layer = map.Layers.FindLayer(name).FirstOrDefault();

        if (layer != null)
        {
            map.Layers.Remove(layer);
        }
    }

    public static T? GetLayer<T>(this Map map, LayerType layerType) where T : ILayer
    {
        return (T?)map.Layers.FirstOrDefault(l => l.Name.Equals(layerType.ToString()));
    }

    public static ILayer? GetLayer(this Map map, LayerType layerType)
    {
        return map.GetLayer<ILayer>(layerType);
    }

    public static void ReplaceLayer(this Map map, ILayer layer, LayerType layerType)
    {
        map.ReplaceLayer(layer, layerType.ToString());
    }

    public static void ReplaceLayer(this Map map, ILayer layer, string name)
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

    public static void ForceUpdate(this Map map)
    {
        var temp = new Layer();
        map.Layers.Add(temp);
        map.Layers.Remove(new[] { temp });
    }

    public static Mapsui.Styles.Color ToMapsuiColor(this System.Drawing.Color color)
    {
        return Mapsui.Styles.Color.FromArgb(255, color.R, color.G, color.B);
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
