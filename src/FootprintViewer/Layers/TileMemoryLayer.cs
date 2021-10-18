using BruTile;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FootprintViewer
{
    public class TileMemoryLayer : MemoryLayer
    {
        private readonly ITileSource _source;

        public TileMemoryLayer(ITileSource source)
        {
            _source = source;
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            var tiles = _source.Schema
                .GetTileInfos(box.ToExtent(), resolution)
                .Select(ToFeature)
                .ToList();

            return tiles;
        }

        private IFeature ToFeature(TileInfo tileInfo)
        {
            var tileData = _source.GetTile(tileInfo);
            return new Feature { Geometry = ToGeometry(tileInfo, tileData) };
        }

        private Raster ToGeometry(TileInfo tileInfo, byte[] tileData)
        {
            return tileData == null ? null : new Raster(new MemoryStream(tileData), tileInfo.Extent.ToBoundingBox());
        }
    }
}
