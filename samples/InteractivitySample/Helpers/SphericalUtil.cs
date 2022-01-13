﻿using Mapsui.Geometries;
using System;
using System.Collections.Generic;

namespace InteractivitySample.Helpers
{
    public static class SphericalUtil
    {
        private const double EARTH_RADIUS = 6371.009; // 6378.137;

        private static double ToRadians(double input)
        {
            return input / 180.0 * Math.PI;
        }

        public static double ComputeSignedArea(IList<Point> path)
        {
            return ComputeSignedArea(path, EARTH_RADIUS);
        }

        private static double ComputeSignedArea(IList<Point> path, double radius)
        {
            int size = path.Count;
            if (size < 3)
            { return 0; }
            double total = 0;
            var prev = path[size - 1];
            double prevTanLat = Math.Tan((Math.PI / 2 - ToRadians(prev.Y)) / 2);
            double prevLng = ToRadians(prev.X);

            foreach (var point in path)
            {
                double tanLat = Math.Tan((Math.PI / 2 - ToRadians(point.Y)) / 2);
                double lng = ToRadians(point.X);
                total += PolarTriangleArea(tanLat, lng, prevTanLat, prevLng);
                prevTanLat = tanLat;
                prevLng = lng;
            }
            return total * (radius * radius);
        }

        private static double PolarTriangleArea(double tan1, double lng1, double tan2, double lng2)
        {
            double deltaLng = lng1 - lng2;
            double t = tan1 * tan2;
            return 2 * Math.Atan2(t * Math.Sin(deltaLng), 1 + t * Math.Cos(deltaLng));
        }

        public static double ComputeDistance(IList<Point> path)
        {
            return ComputeDistance(path, EARTH_RADIUS);
        }

        private static double ComputeDistance(IList<Point> path, double radius)
        {
            int size = path.Count;
            if (size < 2)
            {
                return 0;
            }

            double total = 0;

            Point prevPoint = path[0];

            foreach (var point in path)
            {
                double dist = GetDistance(prevPoint, point);

                total += dist;
                prevPoint = point;
            }

            return total;
        }

        private static double GetDistance(Point p0, Point p1)
        {
            var lat0 = ToRadians(p0.Y);
            var lon0 = ToRadians(p0.X);
            var lat1 = ToRadians(p1.Y);
            var lon1 = ToRadians(p1.X);
            var dlon = lon1 - lon0;
            var d = Math.Pow(Math.Sin((lat1 - lat0) / 2.0), 2.0) + Math.Cos(lat0) * Math.Cos(lat1) * Math.Pow(Math.Sin(dlon / 2.0), 2.0);
            return EARTH_RADIUS * (2.0 * Math.Atan2(Math.Sqrt(d), Math.Sqrt(1.0 - d)));
        }
    }
}