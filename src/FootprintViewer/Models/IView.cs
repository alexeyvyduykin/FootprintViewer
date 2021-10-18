using Mapsui;

namespace FootprintViewer
{
    public interface IView
    {
        Map Map { get; }

        void SetCursorType(CursorType cursorType);
    }
}