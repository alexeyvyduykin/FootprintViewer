﻿using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data.Science
{
    internal class EarthCircle
    {
        public bool IsNorthPoleCover { get; init; }

        public bool IsSouthPoleCover { get; init; }

        public double Angle { get; set; }

        public IEnumerable<IEnumerable<Point>> Borders { get; init; } = new List<List<Point>>();

        // Areas count: 1 or 2
        public IEnumerable<IEnumerable<Point>> Areas { get; init; } = new List<List<Point>>();
    }

    internal static class EarthGeometry
    {
        public static double Bottom { get; set; } = -89.0;

        public static double Rearth { get; set; } = 6371.0;

        public static int SegmentDelta { get; set; } = 4;

        public static IEnumerable<EarthCircle> BuildCircles(double lon, double lat, double[] angles)
        {
            foreach (var angle in angles)
            {
                yield return BuildCircle(lon, lat, angle);
            }
        }

        public static EarthCircle BuildCircle(double lon, double lat, double angle)
        {
            ZonaOb(lon, lat, angle, out double[] lons, out double[] lats, out bool isNorth, out bool isSouth);

            var count = lons.Length;

            var borders = new List<List<Point>>();
            var area = new List<List<Point>>();

            if (isNorth == false && isSouth == false)
            {
                var temp = new List<Point>();
                var areas = new List<Point>[2] { new List<Point>(), new List<Point>() };
                int index = 0;

                double begin = lons[0];
                (double lon, double lat) end = (lons[0], lats[0]);
                for (int i = 0; i < count; i++)
                {
                    double lonn = lons[i] - 180;
                    if (lonn < -180.0)
                    {
                        lonn += 360.0;
                    }

                    begin = lonn;

                    if (Math.Abs(end.lon - begin) > 180.0)
                    {
                        var cutLat = LinearInterpDiscontLat(new Geo2D(begin, lats[i]), new Geo2D(end.lon, end.lat));

                        if (end.lon - begin >= 0)
                        {
                            temp.Add(new Point(180, cutLat));

                            borders.Add(temp);
                            areas[index].AddRange(temp);

                            temp = new List<Point>
                            {
                                new Point(-180, cutLat)
                            };
                        }
                        else
                        {
                            temp.Add(new Point(-180, cutLat));

                            borders.Add(temp);
                            areas[index].AddRange(temp);

                            temp = new List<Point>
                            {
                                new Point(180, cutLat)
                            };
                        }

                        index = Check(index);
                    }

                    temp.Add(new Point(lonn, lats[i]));

                    end = (begin, lats[i]);
                }

                borders.Add(temp);
                areas[index].AddRange(temp);

                if (areas[1].Count == 0)
                {
                    area = new List<List<Point>>() { areas[0] };

                }
                else
                {
                    area = new List<List<Point>>() { areas[0], areas[1] };
                }
            }
            else if (isNorth == true)
            {
                var area1 = new List<Point>();
                var temp = new List<Point>();

                double begin = lons[0];
                (double lon, double lat) end = (lons[0], lats[0]);
                for (int i = 0; i < count; i++)
                {
                    double lonn = lons[i] - 180;
                    if (lonn < -180.0)
                    {
                        lonn += 360.0;
                    }

                    begin = lonn;

                    if (Math.Abs(end.lon - begin) > 180.0)
                    {
                        var cutLat = LinearInterpDiscontLat(new Geo2D(begin, lats[i]), new Geo2D(end.lon, end.lat));

                        if (end.lon - begin >= 0)
                        {

                            temp.Add(new Point(180, cutLat));
                            area1.Add(new Point(180, cutLat));
                            borders.Add(temp);

                            area1.Add(new Point(+180, 90));
                            area1.Add(new Point(-180, 90));

                            temp = new List<Point>
                            {
                                new Point(-180, cutLat)
                            };
                            area1.Add(new Point(-180, cutLat));
                        }
                        else
                        {
                            temp.Add(new Point(-180, cutLat));
                            area1.Add(new Point(-180, cutLat));

                            borders.Add(temp);

                            area1.Add(new Point(-180, 90));
                            area1.Add(new Point(+180, 90));

                            temp = new List<Point>
                            {
                                new Point(180, cutLat)
                            };
                            area1.Add(new Point(180, cutLat));
                        }

                    }

                    temp.Add(new Point(lonn, lats[i]));
                    area1.Add(new Point(lonn, lats[i]));

                    end = (begin, lats[i]);
                }

                borders.Add(temp);
                area = new List<List<Point>>() { area1 };
            }
            else if (isSouth == true)
            {
                var area1 = new List<Point>();
                var temp = new List<Point>();

                double begin = lons[0];
                (double lon, double lat) end = (lons[0], lats[0]);
                for (int i = 0; i < count; i++)
                {
                    double lonn = lons[i] - 180;
                    if (lonn < -180.0)
                    {
                        lonn += 360.0;
                    }

                    begin = lonn;

                    if (Math.Abs(end.lon - begin) > 180.0)
                    {
                        var cutLat = LinearInterpDiscontLat(new Geo2D(begin, lats[i]), new Geo2D(end.lon, end.lat));

                        if (end.lon - begin >= 0)
                        {

                            temp.Add(new Point(180, cutLat));
                            area1.Add(new Point(180, cutLat));
                            borders.Add(temp);

                            area1.Add(new Point(+180, Bottom));
                            area1.Add(new Point(-180, Bottom));

                            temp = new List<Point>
                            {
                                new Point(-180, cutLat)
                            };
                            area1.Add(new Point(-180, cutLat));
                        }
                        else
                        {
                            temp.Add(new Point(-180, cutLat));
                            area1.Add(new Point(-180, cutLat));

                            borders.Add(temp);

                            area1.Add(new Point(+180, Bottom));
                            area1.Add(new Point(-180, Bottom));

                            temp = new List<Point>
                            {
                                new Point(180, cutLat)
                            };
                            area1.Add(new Point(180, cutLat));
                        }
                    }

                    temp.Add(new Point(lonn, lats[i]));
                    area1.Add(new Point(lonn, lats[i]));

                    end = (begin, lats[i]);
                }

                borders.Add(temp);
                area = new List<List<Point>>() { area1 };
            }

            return new EarthCircle()
            {
                IsNorthPoleCover = isNorth,
                IsSouthPoleCover = isSouth,
                Angle = angle,
                Borders = borders,
                Areas = area.Select(s => s.Select(s => s)),
            };
        }

        private static void ZonaOb(double lonDeg, double latDeg, double angleDeg,
            out double[] lons, out double[] lats,
            out bool isNorthPoleCover, out bool isSouthPoleCover)
        {
            double lat = latDeg * ScienceMath.DegreesToRadians;
            double lon = lonDeg * ScienceMath.DegreesToRadians + Math.PI;
            double angle = angleDeg * ScienceMath.DegreesToRadians;

            double xs = Rearth * Math.Cos(angle);
            double zs = Rearth * Math.Sin(angle);

            var segmentCount = 360 / SegmentDelta + 1;

            lats = new double[segmentCount];
            lons = new double[segmentCount];

            for (int i = 0; i <= 360; i += SegmentDelta)
            {
                int j = i / 4;
                double g = i * ScienceMath.DegreesToRadians;
                double xg = xs * Math.Cos(lat) * Math.Cos(-lon) + zs * (-Math.Sin(lat) * Math.Cos(g) * Math.Cos(-lon) + Math.Sin(g) * Math.Sin(-lon));
                double yg = xs * (-Math.Sin(-lon) * Math.Cos(lat)) + zs * (Math.Sin(-lon) * Math.Sin(lat) * Math.Cos(g) + Math.Sin(g) * Math.Cos(-lon));
                double zg = xs * Math.Sin(lat) + zs * Math.Cos(lat) * Math.Cos(g);

                var lon1 = Math.Atan2(yg, xg);
                var lat1 = Math.Asin(zg / Rearth);

                if (lon1 < 0)
                {
                    lon1 += 2 * Math.PI;
                }

                if (lon1 > 2 * Math.PI)
                {
                    lon1 -= 2 * Math.PI;
                }

                lon1 *= ScienceMath.RadiansToDegrees;
                lat1 *= ScienceMath.RadiansToDegrees;

                if (lon1 > 270)
                {
                    lon1 -= 360;
                }

                lons[j] = lon1;
                lats[j] = lat1;
            }

            isNorthPoleCover = (latDeg + angleDeg > 90);
            isSouthPoleCover = (latDeg - angleDeg < -90);
        }

        private static int Check(int index)
        {
            if (index == 0)
            {
                return 1;
            }
            if (index == 1)
            {
                return 0;
            }

            throw new Exception();
        }

        private static double LinearInterpDiscontLat(Geo2D pp1, Geo2D pp2)
        {
            Geo2D p1 = pp1.ToRadians(), p2 = pp2.ToRadians();

            // one longitude should be negative one positive, make them both positive
            double lon1 = p1.Lon, lat1 = p1.Lat, lon2 = p2.Lon, lat2 = p2.Lat;
            if (lon1 > lon2)
            {
                lon2 += 2 * Math.PI; // in radians
            }
            else
            {
                lon1 += 2 * Math.PI;
            }

            return (lat1 + (Math.PI - lon1) * (lat2 - lat1) / (lon2 - lon1));
        }
    }
}