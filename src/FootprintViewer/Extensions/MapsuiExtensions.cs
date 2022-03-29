using BruTile;
using BruTile.MbTiles;
using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.UI;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FootprintViewer
{
    public static class MapsuiExtensions
    {
        public static void SetWorldMapLayer(this Map map, MapResource resource)
        { 
            var layer = CreateWorldMapLayer(resource);
            map.Layers.Replace(nameof(LayerType.WorldMap), layer);
            map.Limiter = new ViewportLimiterKeepWithin { PanLimits = layer.Envelope };
        }

        public static T? GetLayer<T>(this Map map, LayerType layerType) where T : ILayer
        {
            return (T?)map.Layers.FirstOrDefault(l => l.Name.Equals(layerType.ToString()));
        }

        public static void Replace(this LayerCollection collection, string name, ILayer layer)
        {
            int index = 0;
            ILayer? removable = null;

            var count = collection.Count;

            for (int i = 0; i < count; i++)
            {
                if (collection[i].Name.Equals(name) == true)
                {
                    removable = collection[i];
                    index = i;
                    break;
                }
            }

            if (removable != null)
            {
                collection.Remove(removable);

                layer.Name = name;

                collection.Insert(index, layer);
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

            var layer = new TileLayer(mbTilesTileSource)
            {
                Name = nameof(LayerType.WorldMap)
            };

            return layer;
        }
    }
}
