using FootprintViewer.Models;
using SkiaSharp;

namespace FootprintViewer.Builders;

public static class FootprintPreviewBuilder
{
    private static readonly Random _random = new();

    public static FootprintPreview CreateRandom()
    {
        var names = new[] { "02-65-lr_2000-3857-lite", "36-65-ur_2000-3857-lite", "38-50-ll_3857-lite", "38-50-lr_3857-lite", "38-50-ul_3857-lite", "38-50-ur_3857-lite", "41-55-ul_2000-3857-lite", "44-70-ur_2000-3857-lite" };
        var satellites = new[] { "Satellite1", "Satellite2", "Satellite3" };

        var name = names[_random.Next(0, names.Length)].Replace("lite", "").Replace("2000", "").Replace("3857", "").Replace("_", "").Replace("-", "");
        var date = DateTime.UtcNow;

        var unitBitmap = new SKBitmap(1, 1);
        unitBitmap.SetPixel(0, 0, SKColors.White);

        return new FootprintPreview()
        {
            Name = name.ToUpper(),
            Date = date.Date.ToShortDateString(),
            SatelliteName = satellites[_random.Next(0, satellites.Length)],
            SunElevation = _random.Next(0, 91),
            CloudCoverFull = _random.Next(0, 101),
            TileNumber = name.ToUpper(),
            Image = SKImage.FromBitmap(unitBitmap)
        };
    }
}
