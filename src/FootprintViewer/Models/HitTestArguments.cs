using Mapsui.Geometries;

namespace FootprintViewer
{
    public class HitTestArguments
    {
        public HitTestArguments(Point point, double tolerance)
        {
            Point = point;
            Tolerance = tolerance;
        }

        public Point Point { get; private set; }

        public double Tolerance { get; private set; }
    }
}