using BruTile.MbTiles;
using Mapsui;
using Mapsui.Utilities;
using SkiaSharp;
using SQLite;

namespace FootprintViewer.Data.Models;

public class FootprintPreview
{
    private const int _previewWidth = 200;
    private const int _previewHeight = 200;

    public string? Name { get; set; }

    public string? Date { get; set; }

    public string? SatelliteName { get; set; }

    public double SunElevation { get; set; }

    public double CloudCoverFull { get; set; }

    public string? TileNumber { get; set; }

    public string? Path { get; set; }

    public SKImage? Image { get; set; }

    public static Func<IList<string>, IList<object>> Builder =>
        paths =>
        {
            var random = new Random();

            return paths.Select(path =>
            {
                var filename = System.IO.Path.GetFileNameWithoutExtension(path);
                var name = filename?.Split('_').FirstOrDefault();

                return new FootprintPreview()
                {
                    Name = name ?? "DefaultName",
                    Date = DateTime.UtcNow.Date.ToShortDateString(),
                    SatelliteName = $"Satellite{random.Next(1, 6):00}",
                    SunElevation = random.Next(0, 90),
                    CloudCoverFull = random.Next(0, 100),
                    TileNumber = name?.Replace("-", "").ToUpper() ?? "DefaultTileNumber",
                    Path = path,
                    Image = CreatePreviewImage(path),
                };
            }).ToList<object>();
        };

    private static SKImage CreatePreviewImage(string path)
    {
        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

        var layer = new TileMemoryLayer(mbTilesTileSource);

        var extent = mbTilesTileSource.Schema.Extent;

        var viewport = new Viewport
        {
            CenterX = extent.CenterX,
            CenterY = extent.CenterY,
            Width = _previewWidth,
            Height = _previewHeight,
            Resolution = ZoomHelper.CalculateResolutionForWorldSize(extent.Width, extent.Height, _previewWidth, _previewHeight)
        };

        var memoryStream = new Mapsui.Rendering.Skia.MapRenderer().RenderToBitmapStream(viewport, new[] { layer }/*, _backgroundColorMask*/);

        var data = memoryStream!.ToArray();

        return SKImage.FromEncodedData(data);
    }
}
