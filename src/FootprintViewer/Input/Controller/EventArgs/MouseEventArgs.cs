using Mapsui.Geometries;

namespace FootprintViewer
{
    public class MouseEventArgs : InputEventArgs
    {
        public Point Position { get; set; }

        public IView View { get; set; }
    }
}