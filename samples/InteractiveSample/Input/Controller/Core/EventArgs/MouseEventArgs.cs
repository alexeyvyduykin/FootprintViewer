using Mapsui;

namespace InteractiveSample.Input.Controller.Core
{
    public class MouseEventArgs : InputEventArgs
    {
        public MPoint Position { get; set; } = new MPoint();

        public IView? View { get; set; }
    }
}