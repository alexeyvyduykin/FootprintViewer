using Avalonia;

namespace FootprintViewer.Avalonia
{
    public static class PointExtensions
    {
        //public static MPoint ToMapsui(this Point point)
        //{
        //    return new MPoint(point.X, point.Y);
        //}


        public static Mapsui.Geometries.Point ToOldMapsui(this Point point)
        {
            return new Mapsui.Geometries.Point(point.X, point.Y);
        }
    }
}
