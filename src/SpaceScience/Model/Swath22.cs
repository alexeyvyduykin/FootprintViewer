namespace SpaceScience.Model;

public class Swath22
{
    private readonly FactorShiftTrack _factorShiftTrack;
    private readonly GroundTrack _nearTrack;
    private readonly GroundTrack _farTrack;

    public Swath22(Orbit orbit, double lookAngleDEG, double radarAngleDEG, SwathMode mode)
    {
        Orbit = orbit;

        var (near, far) = mode switch
        {
            SwathMode.Middle => (TrackPointDirection.Left, TrackPointDirection.Right),
            SwathMode.Left => (TrackPointDirection.Left, TrackPointDirection.Left),
            SwathMode.Right => (TrackPointDirection.Right, TrackPointDirection.Right),
            _ => throw new NotImplementedException()
        };

        double minLookAngleDeg = lookAngleDEG - radarAngleDEG / 2.0;
        double maxLookAngleDeg = lookAngleDEG + radarAngleDEG / 2.0;

        _factorShiftTrack = new FactorShiftTrack(orbit, minLookAngleDeg, maxLookAngleDeg, mode);

        _nearTrack = new GroundTrack(orbit, _factorShiftTrack, minLookAngleDeg, near);
        _farTrack = new GroundTrack(orbit, _factorShiftTrack, maxLookAngleDeg, far);
    }

    public GroundTrack NearTrack => _nearTrack;

    public GroundTrack FarTrack => _farTrack;

    public bool IsCoverPolis(double latRAD, out double timeFromANToPolis)
    {
        timeFromANToPolis = double.NaN;

        if (_nearTrack.PolisMod(latRAD, out var angleToPolis1) == true
            && _farTrack.PolisMod(latRAD, out var angleToPolis2) == true)
        {
            if (SpaceMath.InRange(Math.PI / 2.0, angleToPolis1, angleToPolis2))
            {
                if (latRAD >= 0.0)
                {
                    timeFromANToPolis = Orbit.Quart1;
                }
                else
                {
                    timeFromANToPolis = Orbit.Quart3;
                }

                return true;
            }
        }

        return false;
    }

    public bool IsCoverPolis(double latRAD)
    {
        if (_nearTrack.PolisMod(latRAD, out var angleToPolis1) == true
            && _farTrack.PolisMod(latRAD, out var angleToPolis2) == true)
        {
            if (SpaceMath.InRange(SpaceMath.HALFPI, angleToPolis1, angleToPolis2))
            {
                return true;
            }
        }
        return false;
    }

    public Orbit Orbit { get; }

    public void CalculateSwathWithLogStep()
    {
        _nearTrack.CalculateTrackWithLogStep(100);
        _farTrack.CalculateTrackWithLogStep(100);
    }
}
