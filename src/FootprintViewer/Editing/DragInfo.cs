using Mapsui.Geometries;
using Mapsui.Providers;

namespace FootprintViewer
{
    public class DragInfo
    {
        public IInteractiveFeature Feature { get; set; }
        public Point Vertex { get; set; }
        public Point StartOffsetToVertex { get; set; }
    }
}