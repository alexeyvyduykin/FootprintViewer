using Mapsui.Geometries;
using Mapsui.Projection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer
{
    public static class ProjectHelper
    {
        public static string ToString(Point worldPosition)
        {
            var p = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
            var lon = (p.X >= 0.0) ? $"{p.X:F5}°E" : $"{Math.Abs(p.X):F5}°W";
            var lat = (p.Y >= 0.0) ? $"{p.Y:F5}°N" : $"{Math.Abs(p.Y):F5}°S";
            return $"{lon} {lat}";
        }
    }
}
