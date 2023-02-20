using NetTopologySuite.Geometries;

namespace DatabaseCreatorSample.Data;

public class GroundStation
{
    public string Name { get; set; }

    public Point Center { get; set; }

    public double[] Angles { get; set; }
}
