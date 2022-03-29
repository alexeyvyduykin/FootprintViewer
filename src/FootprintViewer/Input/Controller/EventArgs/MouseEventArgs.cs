using Mapsui.Geometries;

namespace FootprintViewer.Input
{
    public class MouseEventArgs : InputEventArgs
    {
        public Point? Position { get; set; }

        public IView? View { get; set; }
    }
}