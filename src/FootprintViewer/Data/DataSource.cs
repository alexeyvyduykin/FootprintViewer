using BruTile;
using BruTile.MbTiles;
using FootprintViewer.Models;
using Mapsui;
using Mapsui.Geometries;
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
        private readonly SortedDictionary<string, NetTopologySuite.Geometries.Geometry> _dict = new SortedDictionary<string, NetTopologySuite.Geometries.Geometry>();
        private readonly List<LayerSource> _worldMapSources;
        private readonly List<Footprint> _footprints;
        private Random _random = new Random();
        private readonly string[] _satellites;
        private readonly DateTime _date;
        private readonly SolutionFolder _dataFolder = new SolutionFolder("data");
        private readonly SolutionFolder _userDataFolder = new SolutionFolder("userData");

        public DataSource()
        {
            _worldMapSources = new List<LayerSource>();
            _footprints = new List<Footprint>();

            _satellites = new[] { "Satellite1", "Satellite2", "Satellite3" };
            _date = DateTime.UtcNow;
        
            BuildWorldMaps();
        }

        public IList<LayerSource> WorldMapSources => _worldMapSources;

        private void BuildWorldMaps()
        {
            var layerPath = _dataFolder.GetPath("world.mbtiles", "world");

            var userLayerPath = _userDataFolder.GetPaths("*.mbtiles", "world");

            if (layerPath != null)
            {
                _worldMapSources.Add(new LayerSource() { Name = "WorldDefault", Path = layerPath });
            }

            foreach (var path in userLayerPath)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                _worldMapSources.Add(new LayerSource() { Name = name, Path = path });
            }
        }

        public IEnumerable<Footprint> GetFootprints()
        {          
            BuildFootprintBorders();          
            BuildFootprints();           
            BuildUserFootprints();

            return _footprints;
        }

        private void BuildFootprints()
        {      
            var mbtilesPaths = _dataFolder.GetPaths("*.mbtiles", "footprints");

            foreach (var item in mbtilesPaths)
            {
                var path = item;
                var filename = Path.GetFileNameWithoutExtension(item);
                var name = filename.Split('_').FirstOrDefault();

                var tileNumber = name.Replace("-", "").ToUpper();
                tileNumber += "-Preview";

                if (string.IsNullOrEmpty(name) == false && _dict.ContainsKey(name) == true)
                {
                    var geometry = _dict[name];
                    var points = geometry.Coordinates;
                    var exteriorRing = new LinearRing(points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));
                    var poly = new Polygon(exteriorRing);

                    _footprints.Add(CreateFootprint(tileNumber, path, poly));
                }
            }
        }

        private void BuildUserFootprints()
        {    
            var mbtilesPaths = _userDataFolder.GetPaths("*.mbtiles", "footprints");

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
                    var exteriorRing = new LinearRing(points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));
                    var poly = new Polygon(exteriorRing);

                    _footprints.Add(CreateFootprint(tileNumber, path, poly));
                }
            }
        }

        private Footprint CreateFootprint(string tile, string path, Geometry poly)
        {
            return new Footprint()
            {
                Date = _date.Date.ToShortDateString(),
                SatelliteName = _satellites[_random.Next(0, _satellites.Length)],
                SunElevation = _random.Next(0, 90),
                CloudCoverFull = _random.Next(0, 100),
                TileNumber = tile,
                Path = path,
                Preview = CreatePreviewImage(path),
                Geometry = poly
            };
        }

        private void BuildFootprintBorders()
        {
            _dict.Clear();

            var shapeFileName = _dataFolder.GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff");

            string path = @"C:\Users\User\Desktop\app test\MyTest.txt";

                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine($"shapeFileName={shapeFileName}");                                 
                }
            

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

        private Image CreatePreviewImage(string path)
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
         
            var backgroundColor = new Mapsui.Styles.Color() { R = 33, G = 43, B = 53, A = 255 }; // #212b35

            var bitmap = new MapRenderer().RenderToBitmapStream(viewport, map.Layers, backgroundColor);

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
