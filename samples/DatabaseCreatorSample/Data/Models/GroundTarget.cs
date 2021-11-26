using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseCreatorSample.Data
{
    public enum GroundTargetType
    {
        Point,
        Route,
        Area
    }

    public class GroundTarget
    {
        public string Name { get; set; }

        public GroundTargetType Type { get; set; }
     
        public IEnumerable<Point> Points { get; set; }
    }
}
