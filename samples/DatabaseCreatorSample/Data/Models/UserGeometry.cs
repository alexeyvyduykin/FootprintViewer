using NetTopologySuite.Geometries;

namespace DatabaseCreatorSample.Data
{
    public enum UserGeometryType { Point, Rectangle, Polygon, Circle }

    public class UserGeometry
    {
        public string Name { get; set; }

        public UserGeometryType Type { get; set; }

        public Geometry Geometry { get; set; }
    }
}
