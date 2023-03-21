namespace SpaceScience.Model;

public class FactorShiftTrack
{
    public FactorShiftTrack(Orbit orbit, double gam1DEG, double gam2DEG, SwathMode direction)
    {
        int mdf = 0, ch23, ch4, pls1 = 0, pls2 = 0;

        double gam1 = gam1DEG * SpaceMath.DegreesToRadians;
        double gam2 = gam2DEG * SpaceMath.DegreesToRadians;

        switch (direction)
        {
            case SwathMode.Middle:
                pls1 = -1;
                pls2 = 1;
                break;
            case SwathMode.Left:
                pls1 = pls2 = -1;
                break;
            case SwathMode.Right:
                pls1 = pls2 = 1;
                break;
        }

        double semi_axis = orbit.Semiaxis(SpaceMath.HALFPI);

        double fi1 = SpaceMath.HALFPI - Math.Acos(semi_axis * Math.Sin(gam1) / Constants.Re) - gam1;
        double fi2 = SpaceMath.HALFPI - Math.Acos(semi_axis * Math.Sin(gam2) / Constants.Re) - gam2;

        double i1_90 = orbit.Inclination - fi1 * pls1;
        double i2_90 = orbit.Inclination - fi2 * pls2;
        double di1_90 = Math.Abs(SpaceMath.HALFPI - i1_90);
        double di2_90 = Math.Abs(SpaceMath.HALFPI - i2_90);

        double i1_270 = Math.PI + orbit.Inclination + fi1 * pls1;
        double i2_270 = Math.PI + orbit.Inclination + fi2 * pls2;
        double di1_270 = Math.Abs(3.0 * SpaceMath.HALFPI - i1_270);
        double di2_270 = Math.Abs(3.0 * SpaceMath.HALFPI - i2_270);

        ////////////////////////////////////////////////////////////////////
        double uTr1 = Math.Acos(Math.Cos(fi1) * Math.Cos(SpaceMath.HALFPI));
        double iTr1 = orbit.Inclination - Math.Atan2(Math.Tan(fi1), Math.Sin(SpaceMath.HALFPI)) * pls1;
        double lat1 = Math.Asin(Math.Sin(uTr1) * Math.Sin(iTr1));
        double asinlon1 = Math.Tan(lat1) / Math.Tan(iTr1);
        if (Math.Abs(asinlon1) > 1.0)
            asinlon1 = SpaceMath.Sign(asinlon1);
        double lon1 = Math.Asin(asinlon1) - Constants.Omega * orbit.Quart1;// Period / 4.0;

        double uTr2 = Math.Acos(Math.Cos(fi2) * Math.Cos(SpaceMath.HALFPI));
        double iTr2 = orbit.Inclination - Math.Atan2(Math.Tan(fi2), Math.Sin(SpaceMath.HALFPI)) * pls2;
        double lat2 = Math.Asin(Math.Sin(uTr2) * Math.Sin(iTr2));
        double asinlon2 = Math.Tan(lat2) / Math.Tan(iTr2);
        if (Math.Abs(asinlon2) > 1.0)
            asinlon2 = SpaceMath.Sign(asinlon2);
        double lon2 = Math.Asin(asinlon2) - Constants.Omega * orbit.Quart1;// Period / 4.0;
                                                                           ////////////////////////////////////////////////////////////////////
        ch23 = 0;
        if (lon1 < 0.0 && lon2 < 0.0)
            ch23++;
        if (lon1 > 0.0 && lon2 < 0.0)
            if (di1_90 < di2_90)
                ch23++;
        if (lon1 < 0.0 && lon2 > 0.0)
            if (di1_90 > di2_90)
                ch23++;
        /////////////////////////////////////////////////////////////////////

        uTr1 = Math.Acos(Math.Cos(fi1) * Math.Cos(3.0 * SpaceMath.HALFPI));
        iTr1 = orbit.Inclination - Math.Atan2(Math.Tan(fi1), Math.Sin(3.0 * SpaceMath.HALFPI)) * pls1;
        lat1 = Math.Asin(Math.Sin(uTr1) * Math.Sin(iTr1));
        asinlon1 = Math.Tan(lat1) / Math.Tan(iTr1);
        if (Math.Abs(asinlon1) > 1.0)
            asinlon1 = SpaceMath.Sign(asinlon1);
        lon1 = SpaceMath.TWOPI + Math.Asin(asinlon1) - Constants.Omega * orbit.Quart3;// 3.0 * Period / 4.0;

        uTr2 = Math.Acos(Math.Cos(fi2) * Math.Cos(3.0 * SpaceMath.HALFPI));
        iTr2 = orbit.Inclination - Math.Atan2(Math.Tan(fi2), Math.Sin(3.0 * SpaceMath.HALFPI)) * pls2;
        lat2 = Math.Asin(Math.Sin(uTr2) * Math.Sin(iTr2));
        asinlon2 = Math.Tan(lat2) / Math.Tan(iTr2);
        if (Math.Abs(asinlon2) > 1.0)
            asinlon2 = SpaceMath.Sign(asinlon2);
        lon2 = SpaceMath.TWOPI + Math.Asin(asinlon2) - Constants.Omega * orbit.Quart3;// 3.0 * Period / 4.0;
                                                                                      ///////////////////////////////////////////////////////////////////////////////
        ch4 = ch23;

        if (lon1 > SpaceMath.TWOPI && lon2 > SpaceMath.TWOPI)
            ch4++;
        if (lon1 > SpaceMath.TWOPI && lon2 < SpaceMath.TWOPI)
            if (di1_270 > di2_270)
                ch4++;
        if (lon1 < SpaceMath.TWOPI && lon2 > SpaceMath.TWOPI)
            if (di1_270 < di2_270)
                ch4++;
        ///////////////////////////////////////////////////////////////////////////////////

        if (ch4 == 2)
            mdf = -1;
        if (ch4 == 1)
            mdf = 0;
        if (ch4 == 0)
            mdf = 1;

        //--------------------------------------------------------------------------------------
        int pmdf;
        double modnakl = orbit.InclinationNormal;
        if (modnakl + fi1 < SpaceMath.HALFPI && modnakl + fi2 < SpaceMath.HALFPI)
            pmdf = 1;
        else
            pmdf = 0;
        //---------------------------------
        Offset = mdf;
        Quart23 = ch23;
        Quart4 = ch4;
        Polis = pmdf;
    }

    public int Offset { get; }  // смещение
    public int Quart23 { get; }
    public int Quart4 { get; }
    public int Polis { get; }
}
