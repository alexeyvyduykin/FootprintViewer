using NetTopologySuite.Geometries;

namespace FootprintViewer.Data
{
    public class GroundStation
    {
        public string? Name { get; set; }

        public Point Center { get; set; } = new Point(0, 0);

        public double[] Angles { get; set; } = new double[] { };
    }
}
