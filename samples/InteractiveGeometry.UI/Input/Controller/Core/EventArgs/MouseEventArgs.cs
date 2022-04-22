using Mapsui;

namespace InteractiveGeometry.UI.Input.Core
{
    public class MouseEventArgs : InputEventArgs
    {
        public MPoint Position { get; set; } = new MPoint();

        public IView? View { get; set; }
    }
}