using FootprintViewer.Models;
using Mapsui.Projection;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FootprintViewer
{
    public static class ResourceManager
    {
        private static readonly Dictionary<string, NetTopologySuite.Geometries.Geometry> _dict = new Dictionary<string, NetTopologySuite.Geometries.Geometry>();

        private static void InitDictionary()
        {
            _dict.Clear();

            var shapeFileName = @"C:\Users\User\AlexeyVyduykin\Resources\SAR\mosaics-geotiff\mosaic-tiff-ruonly.shp";

            var shp = new NetTopologySuite.IO.ShapeFile.Extended.ShapeDataReader(shapeFileName);

            foreach (var shapefileFeature in shp.ReadByMBRFilter(shp.ShapefileBounds))
            {
                var obj = shapefileFeature.Attributes["LABEL"];

                if (obj is string name)
                {
                    var geometry = shapefileFeature.Geometry;

                    _dict.Add(name, geometry);
                }
            }

            //var shapeFileName = @"C:\Users\User\AlexeyVyduykin\Resources\SAR\mosaics-geotiff\mosaic-tiff-ruonly.shp";

            //using (var reader = new ShapefileDataReader(shapeFileName, new NetTopologySuite.Geometries.GeometryFactory()))
            //{
            //    int length = reader.DbaseHeader.NumFields;
            //    while (reader.Read())
            //    {
            //        var fdfd = reader["LABEL"];
            //        var geom = reader.Geometry;

            //        int gfgf = 0;
            //    }
            //}
        }

        public static IEnumerable<Footprint> GetFootprints()
        {
            InitDictionary();

            var mbtilesPaths =
            Directory.GetFiles(@"C:\Users\User\AlexeyVyduykin\Resources\footprints", "*.mbtiles").Select(Path.GetFullPath).ToList();

            var list = new List<Footprint>();

            foreach (var item in mbtilesPaths)
            {
                var path = item;
                var filename = Path.GetFileNameWithoutExtension(item);
                var name = filename.Split('_').FirstOrDefault();

                if (string.IsNullOrEmpty(name) == false && _dict.ContainsKey(name) == true)
                {
                    var geometry = _dict[name];
                    var points = geometry.Coordinates;

                    Mapsui.Geometries.LinearRing exteriorRing =
                        new Mapsui.Geometries.LinearRing(points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));
                    var poly = new Mapsui.Geometries.Polygon(exteriorRing);

                    list.Add(new Footprint(filename, path)
                    {
                        Geometry = poly
                    });
                }
            }

            return list;
        }
    }
}
