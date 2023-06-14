using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

//namespace FootprintViewer.Extensions;

//public static class GeometryExtensions
//{
//    public static IList<Coordinate> MainCoordinates(this Geometry geometry)
//    {
//        if (geometry is LineString lineString)
//        {
//            return lineString.Coordinates;
//        }
//        if (geometry is Polygon polygon)
//        {
//            return polygon.ExteriorRing?.Coordinates.ToList() ?? new List<Coordinate>();
//        }
//        if (geometry is Point point)
//        {
//            return new List<Coordinate> { new Coordinate(point.X, point.Y) };
//        }
//        throw new NotImplementedException();
//    }
//}
