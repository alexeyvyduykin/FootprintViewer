using SpaceScience;
using SpaceScience.Model;
using System;
using System.Collections.Generic;

namespace SpaceScienceSample.Models;

public class Swath22
{
    private readonly FactorShiftTrack _factorShiftTrack;
    private readonly double _gam1Deg;
    private readonly double _gam2Deg;
    private readonly SwathMode _swathMode;

    public Swath22(Orbit orbit, double lookAngleDEG, double radarAngleDEG, SwathMode mode)
    {
        Orbit = orbit;

        _swathMode = mode;

        (TrackPointDirection near, TrackPointDirection far) = mode switch
        {
            SwathMode.Middle => (TrackPointDirection.Left, TrackPointDirection.Right),
            SwathMode.Left => (TrackPointDirection.Left, TrackPointDirection.Left),
            SwathMode.Right => (TrackPointDirection.Right, TrackPointDirection.Right),
            _ => throw new NotImplementedException()
        };

        double minLookAngleDeg = lookAngleDEG - radarAngleDEG / 2.0;
        double maxLookAngleDeg = lookAngleDEG + radarAngleDEG / 2.0;
        
        _gam1Deg = minLookAngleDeg;
        _gam2Deg = maxLookAngleDeg;

        _factorShiftTrack = new FactorShiftTrack(orbit, minLookAngleDeg, maxLookAngleDeg, mode);

        NearLine = new FactorTrack(new CustomTrack(orbit, minLookAngleDeg, near), _factorShiftTrack);
        FarLine = new FactorTrack(new CustomTrack(orbit, maxLookAngleDeg, far), _factorShiftTrack);
    }

    public bool IsCoverPolis(double latRAD, ref double timeFromANToPolis)
    {
        double angleToPolis1 = 0.0, angleToPolis2 = 0.0;
        if (NearLine.PolisMod(latRAD, ref angleToPolis1) == true &&
            FarLine.PolisMod(latRAD, ref angleToPolis2) == true)
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
        double angleToPolis1 = 0.0, angleToPolis2 = 0.0;
        if (NearLine.PolisMod(latRAD, ref angleToPolis1) == true &&
            FarLine.PolisMod(latRAD, ref angleToPolis2) == true)
        {
            if (SpaceMath.InRange(SpaceMath.HALFPI, angleToPolis1, angleToPolis2))
            {
                return true;
            }
        }
        return false;
    }

    public Orbit Orbit { get; }

    public IList<(double lonDeg, double latDeg)> GetNearGroundTrack(PRDCTSatellite satellite, int node)
    {
        var orbit = satellite.Orbit;
        var nearAngleDeg = NearLine.Alpha1 * SpaceMath.RadiansToDegrees;

        FactorTrack22 track = new FactorTrack22(orbit, _factorShiftTrack, nearAngleDeg, _swathMode);
       
        track.CalculateTrackWithLogStep(100);

        return track.GetTrack(node, LonConverter);

      //  var track1 = new CustomTrack(Orbit, NearLine.Alpha1 * SpaceMath.RadiansToDegrees, NearLine.Direction);
      //  return GetGroundTrack(track1, satellite, node, converter);
    }

    public IList<(double lonDeg, double latDeg)> GetFarGroundTrack(PRDCTSatellite satellite, int node)
    {
        var orbit = satellite.Orbit;

        var farAngleDeg = FarLine.Alpha1 * SpaceMath.RadiansToDegrees;

        FactorTrack22 track = new FactorTrack22(orbit, _factorShiftTrack, farAngleDeg, _swathMode);
        
        track.CalculateTrackWithLogStep(100);

        return track.GetTrack(node, LonConverter);



        // var track2 = new CustomTrack(Orbit, FarLine.Alpha1 * SpaceMath.RadiansToDegrees, FarLine.Direction);
        //  return GetGroundTrack(track2, satellite, node, converter);
    }
    private static double LonConverter(double lonDeg)
    {
        var value = lonDeg;
        while (value > 180) value -= 360.0;
        while (value < -180) value += 360.0;
        return value;
    }
    private static IList<Geo2D> GetGroundTrack(CustomTrack track, PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D>? converter = null)
    {
        converter ??= SpaceConverters.From0To360;

        var points = new List<Geo2D>();

        var nodes = satellite.Nodes();
        for (int q = 0; q < nodes[node].Quarts.Count; q++)
        {
            for (double t = nodes[node].Quarts[q].TimeBegin; t <= nodes[node].Quarts[q].TimeEnd; t += 5.0)
            {
                var point = track.ContinuousTrack(node, t, satellite.TrueTimePastAN, nodes[node].Quarts[q].Quart);
                points.Add(converter.Invoke(point));
            }
        }
        return points;
    }

    public FactorTrack NearLine { get; private set; }
    public FactorTrack FarLine { get; private set; }
}
