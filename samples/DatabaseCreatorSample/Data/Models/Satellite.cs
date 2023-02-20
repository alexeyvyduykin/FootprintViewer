using System;

namespace DatabaseCreatorSample.Data;

public class Satellite
{
    public string Name { get; set; }

    public double Semiaxis { get; set; }

    public double Eccentricity { get; set; }

    public double InclinationDeg { get; set; }

    public double ArgumentOfPerigeeDeg { get; set; }

    public double LongitudeAscendingNodeDeg { get; set; }

    public double RightAscensionAscendingNodeDeg { get; set; }

    public double Period { get; set; }

    public DateTime Epoch { get; set; }

    public double InnerHalfAngleDeg { get; set; }

    public double OuterHalfAngleDeg { get; set; }
}
