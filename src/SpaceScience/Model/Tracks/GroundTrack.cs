namespace SpaceScience.Model;

public class GroundTrack
{
    private readonly double _earthRotationSec = 86164.09053;
    private readonly FactorShiftTrack _factor;
    private readonly Orbit _orbit;
    private readonly double _angleRad;
    private readonly double _period;
    private readonly int _direction;
    private readonly List<(double lonDeg, double latDeg)> _cacheTrack = new();

    public GroundTrack(Orbit orbit)
    {
        _angleRad = 0.0;
        _orbit = orbit;
        _factor = new FactorShiftTrack(_orbit, 0.0, 0.0, SwathMode.Middle);
        _direction = 0;
        _period = orbit.Period;
    }

    public GroundTrack(Orbit orbit, FactorShiftTrack factor, double angleDeg, TrackPointDirection direction)
    {
        _angleRad = angleDeg * SpaceMath.DegreesToRadians;
        _orbit = orbit;
        _factor = factor;
        _period = orbit.Period;
        _direction = direction switch
        {
            TrackPointDirection.None => 0,
            TrackPointDirection.Left => -1,
            TrackPointDirection.Right => 1,
            _ => 0,
        };
    }

    public List<(double lonDeg, double latDeg)> CacheTrack => _cacheTrack;

    public double NodeOffsetDeg => 360.0 * _factor.Offset;

    public double EarthRotateOffsetDeg => -(_period / _earthRotationSec) * 360.0;

    public void CalculateTrackWithLogStep(int counts)
    {
        _cacheTrack.Clear();

        var slices = (int)Math.Ceiling(counts / 4.0);

        // [0 - 90)
        foreach (var item in LogStep(0.0, 90, slices).SkipLast(1))
        {
            var u = item * SpaceMath.DegreesToRadians;

            var res = ContinuousTrack22(u);

            _cacheTrack.Add(res);
        }

        // [90 - 180)
        foreach (var item in LogStepReverse(180, 90, slices).SkipLast(1))
        {
            var u = item * SpaceMath.DegreesToRadians;

            var res = ContinuousTrack22(u);

            _cacheTrack.Add(res);
        }

        // [180 - 270)
        foreach (var item in LogStep(180, 270, slices).SkipLast(1))
        {
            var u = item * SpaceMath.DegreesToRadians;

            var res = ContinuousTrack22(u);

            _cacheTrack.Add(res);
        }

        // [270 - 360]
        foreach (var item in LogStepReverse(360, 270, slices))
        {
            var u = item * SpaceMath.DegreesToRadians;

            var res = ContinuousTrack22(u);

            _cacheTrack.Add(res);
        }
        return;
    }

    public void CalculateTrack(double dt = 60.0)
    {
        var period = _orbit.Period;

        _cacheTrack.Clear();

        for (double t = 0; t < period; t += dt)
        {
            var u = _orbit.Anomalia(t);

            var res = ContinuousTrack22(u);

            _cacheTrack.Add(res);
        }

        return;
    }

    private static IEnumerable<double> LogStep(double start = 0.0, double end = 1.0, int slices = 10)
    {
        //   List<double> list = new List<double>();

        // I want to create 7 slices in a segment of length = end - start
        // whose extremes are logarithmically distributed:
        //     |         1       |     2    |   3  |  4 | 5 |6 |7|
        //     +-----------------+----------+------+----+---+--+-+
        //   start                                              end

        double scale = (end - start) / Math.Log(1.0 + slices);
        double lower_bound = start;

        double[] arr = new double[slices + 1];

        for (int i = 0; i < slices; ++i)
        {
            // transform to the interval (1,n_slices+1):
            //     1                 2          3      4    5   6  7 8
            //     +-----------------+----------+------+----+---+--+-+
            //   start                                              end

            double upper_bound = start + Math.Log(2.0 + i) * scale;

            // use the extremes in your function
            //my_function(lower_bound, upper_bound);

            //yield return lower_bound;

            //list.Add(lower_bound);
            arr[i] = lower_bound;

            // update
            lower_bound = upper_bound;
        }

        //yield return lower_bound;

        //  list.Add(lower_bound);
        arr[slices] = lower_bound;

        return arr;
    }

    private static IEnumerable<double> LogStepReverse(double start = 0.0, double end = 1.0, int slices = 10)
    {
        //   List<double> list = new List<double>();

        // I want to create 7 slices in a segment of length = end - start
        // whose extremes are logarithmically distributed:
        //     |         1       |     2    |   3  |  4 | 5 |6 |7|
        //     +-----------------+----------+------+----+---+--+-+
        //   start                                              end

        double scale = (end - start) / Math.Log(1.0 + slices);
        double lower_bound = start;

        double[] arr = new double[slices + 1];

        for (int i = 0; i < slices; ++i)
        {
            // transform to the interval (1,n_slices+1):
            //     1                 2          3      4    5   6  7 8
            //     +-----------------+----------+------+----+---+--+-+
            //   start                                              end

            double upper_bound = start + Math.Log(2.0 + i) * scale;

            // use the extremes in your function
            //my_function(lower_bound, upper_bound);

            //yield return lower_bound;

            //list.Add(lower_bound);
            arr[slices - i] = lower_bound;

            // update
            lower_bound = upper_bound;
        }

        //yield return lower_bound;

        //  list.Add(lower_bound);
        arr[0] = lower_bound;

        return arr;
    }

    private (double lonDeg, double latDeg) ContinuousTrack22(double u)
    {
        double semi_axis = (_orbit.Eccentricity == 0.0) ? _orbit.SemimajorAxis : _orbit.Semiaxis(u);
        double angle = SpaceMath.HALFPI - Math.Acos(semi_axis * Math.Sin(_angleRad) / Constants.Re) - _angleRad;
        double uTr = (angle == 0.0) ? u : Math.Acos(Math.Cos(angle) * Math.Cos(u));
        double iTr = _orbit.Inclination - Math.Atan2(Math.Tan(angle), Math.Sin(u)) * _direction;
        double lat = Math.Asin(Math.Sin(uTr) * Math.Sin(iTr));
        double asinlon = Math.Tan(lat) / Math.Tan(iTr);

        if (Math.Abs(asinlon) > 1.0)
        {
            asinlon = SpaceMath.Sign(asinlon);
        }

        double lon = Math.Asin(asinlon);

        if (u >= 0.0 && u < SpaceMath.HALFPI)
        {
            lon = 0 + lon;
        }
        else if (u >= SpaceMath.HALFPI && u < 3.0 * SpaceMath.HALFPI)
        {
            lon = Math.PI - lon - _factor.Quart23 * SpaceMath.TWOPI;
        }
        else if (u >= 3.0 * SpaceMath.HALFPI && u <= SpaceMath.TWOPI)
        {
            lon = SpaceMath.TWOPI + lon - _factor.Quart4 * SpaceMath.TWOPI;
        }

        lon = lon - (_period / _earthRotationSec) * u;

        lon = lon + _orbit.LonAscnNode;

        //   lon = lon + SpaceMath.TWOPI * _factor.Offset;
        return (lon * SpaceMath.RadiansToDegrees, lat * SpaceMath.RadiansToDegrees);
    }

    public bool PolisMod(double lat, out double polis_mod)
    {
        polis_mod = double.NaN;

        double t_polis;
        int err;
        if (lat >= 0.0)
        {
            t_polis = _orbit.TimeHalfPi();
            err = 1;
        }
        else
        {
            t_polis = 3.0 * _orbit.TimeHalfPi();
            err = -1;
        }

        //double per = 3.0 * Orbit.Period / 4.0;

        double fi = CentralAngleFromT(t_polis + _orbit.ArgumentOfPerigee * _orbit.Period / SpaceMath.TWOPI);
        double i = _orbit.Inclination - fi * _direction;

        //double i_deg = Orbit.Inclination * ScienceMath.RadiansToDegrees;
        //double fi_deg = fi * ScienceMath.RadiansToDegrees;

        if (i > SpaceMath.HALFPI)
        {
            i = Math.PI - i;
        }

        if (i <= Math.Abs(lat))
        {
            polis_mod = _orbit.InclinationNormal + fi * _direction * err;
            return true;
        }

        return false;
    }

    private double CentralAngleFromT(double t)
    {
        double u = _orbit.Anomalia(t) + _orbit.ArgumentOfPerigee;
        double a = _orbit.Semiaxis(u);
        return SpaceMath.HALFPI - Math.Acos(a * Math.Sin(_angleRad) / Constants.Re) - _angleRad;
    }
}
