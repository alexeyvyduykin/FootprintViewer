using NetTopologySuite.Geometries;
using System;

namespace FootprintViewer.Data
{
    public enum SatelliteStripDirection
    {
        Left,
        Right
    }

    public class Footprint
    {
        public string? Name { get; set; }

        public string? SatelliteName { get; set; }

        public string? TargetName { get; set; }

        public Point? Center { get; set; }

        public LineString? Points { get; set; }

        public DateTime Begin { get; set; }

        public double Duration { get; set; }

        public int Node { get; set; }

        public SatelliteStripDirection Direction { get; set; }
    }
}
