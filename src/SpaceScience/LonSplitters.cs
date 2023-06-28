namespace SpaceScience;

public static class LonSplitters
{
    /// <summary>
    /// Split to [-180 deg; +180 deg]
    /// </summary>
    public static ILonSplitter Default => new LonSplitter(-180, 180);
}

public interface ILonSplitter
{
    List<List<(double lonDeg, double latDeg)>> Split(List<(double lonDeg, double latDeg)> line);

    List<List<(double lonDeg, double latDeg)>> SplitArea(List<(double lonDeg, double latDeg)> line);
}

internal class LonSplitter : ILonSplitter
{
    private readonly double _lonLeft;
    private readonly double _lonRight;

    internal LonSplitter(double lonMin, double lonMax)
    {
        _lonLeft = lonMin;
        _lonRight = lonMax;
    }

    public List<List<(double lonDeg, double latDeg)>> Split(List<(double lonDeg, double latDeg)> line)
    {
        var res = new List<List<(double, double)>>();

        var temp = new List<(double, double)>();

        var (prevLonDeg, prevLatDeg) = line.FirstOrDefault();
        var (_, prevNum, _) = LonConverter(prevLonDeg);

        foreach (var (curLonDeg, curLatDeg) in line)
        {
            var (lonDegConvert, num, edge) = LonConverter(curLonDeg);

            if (num != prevNum)
            {
                var cutLatDeg = LinearInterpDiscontLat(prevLonDeg, prevLatDeg, lonDegConvert, curLatDeg);

                if (edge is { })
                {
                    if (edge == _lonRight)
                    {
                        temp.Add((_lonRight, cutLatDeg));
                        res.Add(temp);
                        temp = new() { (_lonLeft, cutLatDeg), (lonDegConvert, curLatDeg) };
                    }
                    else if (edge == _lonLeft)
                    {
                        temp.Add((_lonLeft, cutLatDeg));
                        res.Add(temp);
                        temp = new() { (_lonRight, cutLatDeg), (lonDegConvert, curLatDeg) };
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                temp.Add((lonDegConvert, curLatDeg));
            }

            prevLonDeg = lonDegConvert;
            prevLatDeg = curLatDeg;
            prevNum = num;
        }

        res.Add(temp);

        return res;
    }

    private static double LinearInterpDiscontLat(double lonDeg1, double latDeg1, double lonDeg2, double latDeg2)
    {
        if (lonDeg1 > lonDeg2)
        {
            lonDeg2 += 360.0;
        }
        else
        {
            lonDeg1 += 360.0;
        }

        return (latDeg1 + (180.0 - lonDeg1) * (latDeg2 - latDeg1) / (lonDeg2 - lonDeg1));
    }

    private (double lonDegConvert, int num, double? lonDegEdge) LonConverter(double lonDeg)
    {
        int num = 0;

        if (lonDeg > _lonRight)
        {
            while (lonDeg > _lonRight)
            {
                lonDeg -= 360.0;
                num++;
            }

            return (lonDeg, num, _lonRight);
        }

        if (lonDeg < _lonLeft)
        {
            while (lonDeg < _lonLeft)
            {
                lonDeg += 360.0;
                num++;
            }

            return (lonDeg, num, _lonLeft);
        }

        return (lonDeg, num, null);
    }

    public List<List<(double lonDeg, double latDeg)>> SplitArea(List<(double lonDeg, double latDeg)> line)
    {
        var count = line.Count;
        var vertices1 = new List<(double, double)>();
        var vertices2 = new List<(double, double)>();
        var vertices = vertices1;
        var swap = false;

        for (int i = 0; i < count; i++)
        {
            var p1 = line[i];
            var p2 = (i == count - 1) ? line[0] : line[i + 1];

            var (p1LonDegConvert, num1, edge1) = LonConverter(p1.lonDeg);
            var (p2LonDegConvert, num2, edge2) = LonConverter(p2.lonDeg);

            vertices.Add((p1LonDegConvert, p1.latDeg));

            if (num1 != num2)
            {
                var cutLat = LinearInterpDiscontLat(p1LonDegConvert, p1.latDeg, p2LonDegConvert, p2.latDeg);

                if (edge1 == _lonLeft || edge2 == _lonLeft) // -180 cutting
                {
                    vertices.Add(((swap == false) ? _lonLeft : _lonRight, cutLat));

                    vertices = (vertices == vertices1) ? vertices2 : vertices1;

                    vertices.Add(((swap == false) ? _lonRight : _lonLeft, cutLat));
                }

                if (edge1 == _lonRight || edge2 == _lonRight) // +180 cutting
                {
                    vertices.Add(((swap == false) ? _lonRight : _lonLeft, cutLat));

                    vertices = (vertices == vertices1) ? vertices2 : vertices1;

                    vertices.Add(((swap == false) ? _lonLeft : _lonRight, cutLat));
                }

                swap = !swap;
            }
        }

        if (vertices2.Count != 0) // multipolygon
        {
            return new() { vertices1, vertices2 };
        }
        else
        {
            return new() { vertices1 };
        }
    }
}