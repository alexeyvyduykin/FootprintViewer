namespace SpaceScience.Model;

public class PRDCTSensor
{
    private readonly double _verticalHalfAngleDEG;
    private readonly double _rollAngleDEG;

    internal PRDCTSensor(double verticalHalfAngleDEG, double rollAngleDEG)
    {
        _verticalHalfAngleDEG = verticalHalfAngleDEG;
        _rollAngleDEG = rollAngleDEG;
    }

    public double VerticalHalfAngleDEG => _verticalHalfAngleDEG;

    public double RollAngleDEG => _rollAngleDEG;
}
