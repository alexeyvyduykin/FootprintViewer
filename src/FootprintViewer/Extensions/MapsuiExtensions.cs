using BruTile;
using BruTile.MbTiles;
using FootprintViewer.Data;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.UI;
using SQLite;
using System.Linq;

namespace FootprintViewer
{
    public static class MapsuiExtensions
    {
        public static void SetWorldMapLayer(this Map map, MapResource resource)
        {
            var layer = CreateWorldMapLayer(resource);
            map.ReplaceLayer(layer, LayerType.WorldMap);
            map.Limiter = new ViewportLimiterKeepWithin { PanLimits = layer.Envelope };
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

            var area = mbTilesTileSource.Schema.Extent.ToBoundingBox();
            var delta = area.Width / 2.0;

            //double minX, double minY, double maxX, double maxY
            var limiter = new BoundingBox(area.Left - delta, area.Bottom, area.Right + delta, area.Top);

            return new TileLayer(mbTilesTileSource);
        }
    }
}
