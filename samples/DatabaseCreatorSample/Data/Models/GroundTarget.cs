using NetTopologySuite.Geometries;

namespace DatabaseCreatorSample.Data;

public class GroundTarget
{
    public string Name { get; set; }

    public GroundTargetType Type { get; set; }

    public Geometry Points { get; set; }
}
