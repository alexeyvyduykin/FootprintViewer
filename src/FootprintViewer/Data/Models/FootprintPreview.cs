using SkiaSharp;

namespace FootprintViewer.Data
{
    public class FootprintPreview
    {
        public string? Name { get; set; }

        public string? Date { get; set; }

        public string? SatelliteName { get; set; }

        public double SunElevation { get; set; }

        public double CloudCoverFull { get; set; }

        public string? TileNumber { get; set; }

        public string? Path { get; set; }

        public SKImage? Image { get; set; }
    }
}
