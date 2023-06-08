namespace SpaceScience.Model;

public class Swath
{
    private readonly FactorShiftTrack _factorShiftTrack;
    private readonly GroundTrack _nearTrack;
    private readonly GroundTrack _farTrack;
    private readonly SwathDirection _direction;

    public Swath(Orbit orbit, double lookAngleDEG, double radarAngleDEG, SwathDirection direction)
    {
        Orbit = orbit;

        _direction = direction;

        var (near, far) = direction switch
        {
            SwathDirection.Middle => (TrackDirection.Left, TrackDirection.Right),
            SwathDirection.Left => (TrackDirection.Left, TrackDirection.Left),
            SwathDirection.Right => (TrackDirection.Right, TrackDirection.Right),
            _ => throw new NotImplementedException()
        };

        double minLookAngleDeg = lookAngleDEG - radarAngleDEG / 2.0;
        double maxLookAngleDeg = lookAngleDEG + radarAngleDEG / 2.0;

        _factorShiftTrack = new FactorShiftTrack(orbit, minLookAngleDeg, maxLookAngleDeg, direction);

        _nearTrack = new GroundTrack(orbit, _factorShiftTrack, minLookAngleDeg, near);
        _farTrack = new GroundTrack(orbit, _factorShiftTrack, maxLookAngleDeg, far);
    }

    public GroundTrack NearTrack => _nearTrack;

    public GroundTrack FarTrack => _farTrack;

    public SwathDirection Direction => _direction;

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
                    timeFromANToPolis = Orbit.TimeHalfPi();// Orbit.Quart1;
                }
                else
                {
                    timeFromANToPolis = Orbit.Period - Orbit.TimeHalfPi();// Orbit.Quart3;
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

    public void CalculateSwathOnInterval(double t0, double t1, double dt)
    {
        _nearTrack.CalculateTrackOnTimeInterval(t0, t1, dt);
        _farTrack.CalculateTrackOnTimeInterval(t0, t1, dt);
    }
}
