using Mapsui;

namespace InteractiveGeometry.UI
{
    public interface IView
    {
        Map Map { get; }

        void SetCursor(CursorType cursorType, string info = "");
    }
}