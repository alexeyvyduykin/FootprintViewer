using BruTile;
using BruTile.MbTiles;
using FootprintViewer.FileSystem;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Rendering.Skia;
using Mapsui.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace FootprintViewer.Data.Sources
{
    public class FootprintPreviewDataSource : IFootprintPreviewDataSource
    {
        private readonly Random _random = new Random();
        private readonly DateTime _date;
        private readonly SolutionFolder _dataFolder;
        private readonly string _file;
        private readonly string? _subFolder;
        private readonly string? _searchPattern;
        //private readonly Mapsui.Styles.Color _backgroundColorMask = new Mapsui.Styles.Color() { R = 33, G = 43, B = 53, A = 255 }; // #212b35
        private readonly Mapsui.Styles.Color _backgroundColorMask = new Mapsui.Styles.Color() { R = 66, G = 66, B = 66, A = 255 }; // #424242                                                                                                                                

        public FootprintPreviewDataSource(string file, string folder, string? subFolder = null)
        {
            _file = file;

            if (file.Contains("*.mbtiles") == true)
            {
                _searchPattern = file;
            }

            _subFolder = subFolder;

            _dataFolder = new SolutionFolder(folder);

            _date = DateTime.UtcNow;
        }

        public IEnumerable<FootprintPreview> GetFootprintPreviews()
        {
            var list = new List<FootprintPreview>();

            IEnumerable<string?> paths;

            if (_searchPattern == null)
            {
                paths = new[] { _dataFolder.GetPath(_file, _subFolder) };
            }
            else
            {
                paths = _dataFolder.GetPaths(_searchPattern, _subFolder);
            }
         
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path) == false)
                {
                    var filename = Path.GetFileNameWithoutExtension(path);
                    var name = filename?.Split('_').FirstOrDefault();

                    if (string.IsNullOrEmpty(filename) == false && string.IsNullOrEmpty(name) == false)
                    {
                        var tileNumber = name.Replace("-", "").ToUpper();

                        //var geometry = _dict[name];
                        //var points = geometry.Coordinates;
                        //var exteriorRing = new LinearRing(points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y)));
                        //var poly = new Polygon(exteriorRing);

                        list.Add(new FootprintPreview()
                        {
                            Name = filename,
                            Date = _date.Date.ToShortDateString(),
                            SatelliteName = $"Satellite{_random.Next(1, 6):00}",
                            SunElevation = _random.Next(0, 90),
                            CloudCoverFull = _random.Next(0, 100),
                            TileNumber = tileNumber,
                            Path = path,
                            Image = CreatePreviewImage(path),
                            //Geometry = poly
                        });
                    }
                }
            }

            return list;
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

            var bitmap = new MapRenderer().RenderToBitmapStream(viewport, map.Layers, _backgroundColorMask);

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
