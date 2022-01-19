using ReactiveUI;
using System.Drawing;

namespace FootprintViewer.ViewModels
{
    public class FootprintPreview : ReactiveObject
    {
        public FootprintPreview()
        {
        }

        public string Date { get; set; }

        public string SatelliteName { get; set; }

        public double SunElevation { get; set; }

        public double CloudCoverFull { get; set; }

        public string TileNumber { get; set; }

        public string? Path { get; set; }

        public Mapsui.Geometries.Geometry? Geometry { get; set; }

        public Image Image { get; set; }
    }
}
