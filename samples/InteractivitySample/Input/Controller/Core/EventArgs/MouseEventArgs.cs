using Mapsui.Geometries;

namespace InteractivitySample.Input.Controller.Core
{
    public class MouseEventArgs : InputEventArgs
    {
        public Point Position { get; set; } = new Point();

        public IView? View { get; set; }
    }
}