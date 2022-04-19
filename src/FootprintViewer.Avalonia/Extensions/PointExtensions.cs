using Avalonia;
using Mapsui;

namespace FootprintViewer.Avalonia
{
    public static class PointExtensions
    {
        //public static MPoint ToMapsui(this Point point)
        //{
        //    return new MPoint(point.X, point.Y);
        //}

        public static MPoint ToOldMapsui(this Point point)
        {
            return new MPoint(point.X, point.Y);
        }
    }
}
