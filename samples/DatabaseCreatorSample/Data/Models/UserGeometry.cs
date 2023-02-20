using NetTopologySuite.Geometries;

namespace DatabaseCreatorSample.Data;

public class UserGeometry
{
    public string Name { get; set; }

    public UserGeometryType Type { get; set; }

    public Geometry Geometry { get; set; }
}
