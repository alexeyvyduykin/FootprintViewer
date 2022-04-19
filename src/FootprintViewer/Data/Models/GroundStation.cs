using NetTopologySuite.Geometries;
using System;

namespace FootprintViewer.Data
{
    public class GroundStation
    {
        public string? Name { get; set; }

        public Point Center { get; set; } = new Point(0, 0);

        public double[] Angles { get; set; } = Array.Empty<double>();
    }
}
