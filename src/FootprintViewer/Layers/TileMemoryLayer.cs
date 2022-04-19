using BruTile;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Providers;
using Mapsui.Tiling.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FootprintViewer.Layers
{
    public class TileMemoryLayer : MemoryLayer
    {
        private readonly ITileSource _source;

        public TileMemoryLayer(ITileSource source)
        {
            _source = source;
        }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            var tiles = _source.Schema
                .GetTileInfos(box.ToExtent(), resolution)
                .Select(ToFeature)
                .ToList();

            return tiles;
        }

        private IFeature ToFeature(TileInfo tileInfo)
        {
            var tileData = _source.GetTileAsync(tileInfo).Result;

            //var tileData = _source.GetTile(tileInfo);
            return new RasterFeature(ToGeometry(tileInfo, tileData));
        }

        private MRaster? ToGeometry(TileInfo tileInfo, byte[]? tileData)
        {
            return tileData == null ? null : new MRaster(/*new MemoryStream(*/tileData/*)*/, tileInfo.Extent.ToMRect());
        }
    }
}
