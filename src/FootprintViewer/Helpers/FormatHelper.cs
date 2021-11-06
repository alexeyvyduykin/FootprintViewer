using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer
{
    public static class FormatHelper
    {
        public static string ToDistance(double distance)
        {
            return (distance >= 1) ? $"{distance:N2} km" : $"{distance * 1000.0:N2} m";
        }

        public static string ToArea(double area)
        {
            return $"{area:N2} km²";
        }

        public static string ToCoordinate(double lon, double lat)
        {
            var lonStr = (lon >= 0.0) ? $"{lon:F5}°E" : $"{Math.Abs(lon):F5}°W";
            var latStr = (lat >= 0.0) ? $"{lat:F5}°N" : $"{Math.Abs(lat):F5}°S";
            return $"{lonStr} {latStr}";
        }

        public static string ToScale(double scale)
        {
            return $"1:{scale:N0}";
        }

        public static string ToPercent(double value)
        {
            return $"{value}%";
        }

        public static string ToDegree(double value)
        {
            return $"{value}°";
        }
    }
}
