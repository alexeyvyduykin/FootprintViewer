﻿using FootprintViewer.Data.Models;
using SpaceScience;
using SpaceScience.Model;

namespace FootprintViewer.Data.Extensions;

public static class SatelliteExtensions
{
    public static PRDCTSatellite ToPRDCTSatellite(this Satellite satellite)
    {
        var a = satellite.Semiaxis;
        var ecc = satellite.Eccentricity;
        var inclDeg = satellite.InclinationDeg;
        var argOfPerDeg = satellite.ArgumentOfPerigeeDeg;
        var lonANDeg = satellite.LongitudeAscendingNodeDeg;
        var raanDeg = satellite.RightAscensionAscendingNodeDeg;
        var period = satellite.Period;
        var epoch = satellite.Epoch;

        var factory = new SpaceScienceFactory();

        var orbit = factory.CreateOrbit(a, ecc, inclDeg, argOfPerDeg, lonANDeg, raanDeg, period, epoch);

        return factory.CreateSatellite(orbit, 1);
    }

    public static PRDCTSensor ToPRDCTSensor(this Satellite satellite)
    {
        var factory = new SpaceScienceFactory();

        return factory.CreateSensor(satellite.LookAngleDeg, satellite.RadarAngleDeg);
    }
}