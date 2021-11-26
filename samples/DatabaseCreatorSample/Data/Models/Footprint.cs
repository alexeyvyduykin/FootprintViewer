using Mapsui.Geometries;
using System;
using System.Collections.Generic;

namespace DatabaseCreatorSample.Data
{
    public class Footprint
    {
        public string Name { get; set; }

        public string SatelliteName { get; set; }

        public string TargetName { get; set; }

        public Point Center { get; set; }

        public IEnumerable<Point> Border { get; set; } 

        public DateTime Begin { get; set; }

        public double Duration { get; set; }

        public int Node { get; set; }

        public int Direction { get; set; }
    }
}
