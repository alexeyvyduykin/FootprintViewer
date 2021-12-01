using System.Collections.Generic;
using NetTopologySuite.Geometries;

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

        public Geometry Points { get; set; }
    }
}
