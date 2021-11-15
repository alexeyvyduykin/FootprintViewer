using BruTile;
using BruTile.MbTiles;
using FootprintViewer.Models;
using Mapsui;
using Mapsui.Projection;
using Mapsui.Rendering.Skia;
using Mapsui.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{

    public interface IDataSource
    {
        IEnumerable<Footprint> GetFootprints();

        IList<LayerSource> WorldMapSources { get; }
    }

    public class DataSource : IDataSource
    {
        private readonly Dictionary<string, NetTopologySuite.Geometries.Geometry> _dict = new Dictionary<string, NetTopologySuite.Geometries.Geometry>();
        private readonly List<LayerSource> _worldMapSources;

        public DataSource()
        {
            var layerPath = @"..\\..\\..\\..\\..\\data\\world\\world.mbtiles";
            var userLayerPath = Directory.GetFiles(@"..\\..\\..\\..\\..\\userData\\world", "*.mbtiles").Select(Path.GetFullPath).ToList();

            _worldMapSources = new List<LayerSource>();

            _worldMapSources.Add(new LayerSource() { Name = "WorldDefault", Path = layerPath });

            foreach (var path in userLayerPath)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                _worldMapSources.Add(new LayerSource() { Name = name, Path = path });
            }
        }

        public IList<LayerSource> WorldMapSources => _worldMapSources;

        private void InitDictionary()
        {
            _dict.Clear();

            var shapeFileName = @"..\\..\\..\\..\\..\\data\\mosaics-geotiff\\mosaic-tiff-ruonly.shp";

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
        }

        public IEnumerable<Footprint> GetFootprints()
        {
            InitDictionary();

            var mbtilesPaths = Directory.GetFiles(@"..\\..\\..\\..\\..\\data\\footprints", "*.mbtiles").Select(Path.GetFullPath).ToList();

            var list = new List<Footprint>();

            Random random = new Random();

            var date = DateTime.UtcNow;
            var satellites = new[] { "Satellite1", "Satellite2", "Satellite3" };

            foreach (var item in mbtilesPaths)
            {
                var path = item;
                var filename = Path.GetFileNameWithoutExtension(item);
                var name = filename.Split('_').FirstOrDefault();

                var tileNumber = name.Replace("-", "").ToUpper();

                if (string.IsNullOrEmpty(name) == false && _dict.ContainsKey(name) == true)
                {
                    var geometry = _dict[name];
                    var points = geometry.Coordinates;

                    Mapsui.Geometries.LinearRing exteriorRing =
                        new Mapsui.Geometries.LinearRing(points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));
                    var poly = new Mapsui.Geometries.Polygon(exteriorRing);

                    list.Add(new Footprint()
                    {
                        Date = date.Date.ToShortDateString(),
                        SatelliteName = satellites[random.Next(0, satellites.Length)],
                        SunElevation = random.Next(0, 90),
                        CloudCoverFull = random.Next(0, 100),
                        TileNumber = tileNumber,

                        Path = path,
                        Image0 = CreateMbTilesLayer(path),
                        Geometry = poly
                    });
                }
            }

            return list;
        }

        private Image CreateMbTilesLayer(string path)
        {
            var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

            var layer = new TileMemoryLayer(mbTilesTileSource);

            var map = new Map();

            map.Layers.Add(layer);

            var area = mbTilesTileSource.Schema.Extent.ToBoundingBox();

            var viewport = new Viewport
            {
                Center = area.Centroid,
                Width = 200,
                Height = 200,
                Resolution = ZoomHelper.DetermineResolution(area.Width, area.Height, 200, 200)
            };

            var bitmap = new MapRenderer().RenderToBitmapStream(viewport, map.Layers, map.BackColor);

            // var imageSource = new BitmapImage();
            // imageSource.BeginInit();
            // imageSource.StreamSource = bitmap;
            // imageSource.EndInit();
            // imageSource.Freeze();

            // Image1 = new System.Drawing.Bitmap(200, 200, PixelFormat.Format24bppRgb);

            return Image.FromStream(bitmap);

            //   image.Save(@"C:/Users/User/AlexeyVyduykin/Resources/ttttttttttttt.png");

            //   using (MemoryStream memory = bitmap)
            //   {
            //       Image1.Save(memory, ImageFormat.Png);               
            //       memory.Position = 0;
            //   }

            //return imageSource;
        }
    }
}
