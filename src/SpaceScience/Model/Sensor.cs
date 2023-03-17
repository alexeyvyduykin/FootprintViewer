namespace SpaceScience.Model;

public class PRDCTSensor
{
    private readonly double _radarAngleDeg;
    private readonly double _lookAngleDeg;
    private readonly double _minLookAngleDeg;
    private readonly double _maxLookAngleDeg;

    internal PRDCTSensor(double lookAngleDeg, double radarAngleDeg)
    {
        _lookAngleDeg = lookAngleDeg;
        _radarAngleDeg = radarAngleDeg;

        _minLookAngleDeg = lookAngleDeg - radarAngleDeg / 2.0;
        _maxLookAngleDeg = lookAngleDeg + radarAngleDeg / 2.0;
    }

    public double LookAngleDeg => _lookAngleDeg;

    public double RadarAngleDeg => _radarAngleDeg;

    public double MinLookAngleDeg => _minLookAngleDeg;

    public double MaxLookAngleDeg => _maxLookAngleDeg;
}
