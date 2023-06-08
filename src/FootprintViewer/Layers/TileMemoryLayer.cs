using BruTile;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling.Extensions;
using Mapsui.Tiling.Layers;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers;

public class TileMemoryLayer : TileLayer
{
    private readonly ITileSource _source;

    public TileMemoryLayer(ITileSource source) : base(source)
    {
        _source = source;
    }

    public override IEnumerable<IFeature> GetFeatures(MRect? box, double resolution)
    {
        var tiles = _source.Schema
            .GetTileInfos(box!.ToExtent(), resolution)
            .Select(ToFeature)
            .ToList();

        return tiles;
    }

    private IFeature ToFeature(TileInfo tileInfo)
    {
        var tileData = _source.GetTileAsync(tileInfo).Result;
        return new RasterFeature(ToGeometry(tileInfo, tileData));
    }

    private static MRaster? ToGeometry(TileInfo tileInfo, byte[]? tileData)
    {
        return tileData == null ? null : new MRaster(tileData, tileInfo.Extent.ToMRect());
    }
}
