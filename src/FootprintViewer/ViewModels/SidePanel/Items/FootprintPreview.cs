using ReactiveUI;
using SkiaSharp;

namespace FootprintViewer.ViewModels
{
    public class FootprintPreview : ReactiveObject, IViewerItem
    {
        public FootprintPreview(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string? Date { get; set; }

        public string? SatelliteName { get; set; }

        public double SunElevation { get; set; }

        public double CloudCoverFull { get; set; }

        public string? TileNumber { get; set; }

        public string? Path { get; set; }

        public SKImage? Image { get; set; }

        public bool IsShowInfo { get; set; }
    }
}
