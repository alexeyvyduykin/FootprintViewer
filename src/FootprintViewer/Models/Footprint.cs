using BruTile;
using BruTile.MbTiles;
using Mapsui;
using Mapsui.Rendering.Skia;
using Mapsui.Utilities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SQLite;
using System.Drawing;

namespace FootprintViewer.Models
{
    public class Footprint : ReactiveObject
    {
        public Footprint(string name, string path)
        {
            Name = name;
            Path = path;

            //Image0 = 
            CreateMbTilesLayer(path);
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public Mapsui.Geometries.Geometry? Geometry { get; set; }

        [Reactive]
        public Image Image0 { get; set; }

        public void CreateMbTilesLayer(string path)
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

            Image0 = Image.FromStream(bitmap);

            //   image.Save(@"C:/Users/User/AlexeyVyduykin/Resources/ttttttttttttt.png");

            //   using (MemoryStream memory = bitmap)
            //   {
            //       Image1.Save(memory, ImageFormat.Png);               
            //       memory.Position = 0;
            //   }

            //return imageSource;
        }

        //image.Source = await Task.Run(() => GetBitmap(pixels));
    }
}
