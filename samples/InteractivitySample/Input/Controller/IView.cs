using Mapsui;

namespace InteractivitySample.Input.Controller
{
    public interface IView
    {
        Map Map { get; }

        void SetCursor(CursorType cursorType, string info = "");
    }
}