using System;

namespace SpaceScienceSample.Models;

public class GoldenSectionSearch
{
    public double invphi = (Math.Sqrt(5.0) - 1) / 2.0;
    public double invphi2 = (3 - Math.Sqrt(5.0)) / 2.0;

    public double Search(Func<double, double> f, double a, double b, double tol = 1e-5)
    {
        //var step = (b - a) / 4.0;

        //var ival1 = (a, a + step);
        //var ival2 = (a + step, a + 2 * step);
        //var ival3 = (a + 2 * step, a + 3 * step);
        //var ival4 = (a + 3 * step, b);

        //var min1 = Math.Min(ival1.Item1, ival1.Item2);
        //var min2 = Math.Min(ival2.Item1, ival2.Item2);
        //var leftMin = Math.Min(min1, min2);
        //var min3 = Math.Min(ival3.Item1, ival3.Item2);
        //var min4 = Math.Min(ival4.Item1, ival4.Item2);
        //var rightMin = Math.Min(min3, min4);

        //if (leftMin < rightMin)
        //{
        //   // b = a + 2 * step;
        //}
        //else
        //{
        //   // a = a + 2 * step;
        //}

        var a1 = a;
        var b1 = a + (b - a) / 2.0;

        var a2 = a + (b - a) / 2.0;
        var b2 = b;


        var arr1 = gss(f, a1, b1, tol);
        var arr2 = gss(f, a2, b2, tol);

        var res1 = arr1[0] + (arr1[1] - arr1[0]) / 2.0;
        var res2 = arr2[0] + (arr2[1] - arr2[0]) / 2.0;

        var angle1 = f(res1);
        var angle2 = f(res2);

        return (angle1 > angle2) ? res2 : res1;

        // return arr[0] + (arr[1] - arr[0]) / 2.0;
    }

    // Returns subinterval of [a,b] containing minimum of f
    private double[] gss(Func<double, double> f, double a, double b, double tol)
    {
        return gss(f, a, b, tol, b - a, true, 0, 0, true, 0, 0);
    }

    private double[] gss(Func<double, double> f, double a, double b, double tol,
        double h, bool noC, double c, double fc,
        bool noD, double d, double fd)
    {
        if (Math.Abs(h) <= tol)
        {
            return new double[] { a, b };
        }

        if (noC)
        {
            c = a + invphi2 * h;
            fc = f(c);
        }

        if (noD)
        {
            d = a + invphi * h;
            fd = f(d);
        }

        if (fc < fd) // fc > fd to find the maximum
        {
            return gss(f, a, d, tol, h * invphi, true, 0, 0, false, c, fc);
        }
        else
        {
            return gss(f, c, b, tol, h * invphi, false, d, fd, true, 0, 0);
        }
    }
}