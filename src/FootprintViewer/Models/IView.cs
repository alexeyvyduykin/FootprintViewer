using FootprintViewer.Input;
using Mapsui;

namespace FootprintViewer
{
    public interface IView
    {
        Map? Map { get; }

        void SetCursor(CursorType cursorType);
    }
}