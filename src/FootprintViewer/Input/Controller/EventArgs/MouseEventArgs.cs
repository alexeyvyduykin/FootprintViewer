using Mapsui;

namespace FootprintViewer.Input
{
    public class MouseEventArgs : InputEventArgs
    {
        public MPoint? Position { get; set; }

        public IView? View { get; set; }
    }
}