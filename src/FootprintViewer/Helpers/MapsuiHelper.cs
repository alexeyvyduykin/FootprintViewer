using BruTile;
using BruTile.MbTiles;
using FootprintViewer.Layers;
using Mapsui.Layers;
using SQLite;

namespace FootprintViewer
{
    public static class MapsuiHelper
    {
        public static ILayer CreateTileLayer(ITileSource tileSource, string? name = null)
        {
            return new TileLayer(tileSource) { Name = name ?? tileSource.Name };
        }

        public static TileLayer CreateMbTilesLayer(string path)
        {
            TileSchema schema = new TileSchema()
            {
                Srs = "EPSG:3857"// "EPSG:4326"
            };

            var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));


            var mbTilesLayer = new TileLayer(mbTilesTileSource);
            return mbTilesLayer;
        }

        public static TileMemoryLayer CreatedTileMemoryLayer(string path)
        {
            TileSchema schema = new TileSchema()
            {
                //  Srs = "EPSG:3857"// "EPSG:4326"
            };

            var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true)/*, schema*/);

            var layer = new TileMemoryLayer(mbTilesTileSource);

            return layer;
        }
    }
}
