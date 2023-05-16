using FootprintViewer.Helpers;
using FootprintViewer.Logging;
using Mapsui;
using Mapsui.Layers;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{
    public static class MapHelper
    {
        //Earth radius in meters.
        private const double EarthRadius = 6378137;

        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double MinLongitude = -180;
        private const double MaxLongitude = 180;

        public static double GroundResolution(double latitude, double zoom, int tileSize)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            return Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * EarthRadius / MapSize(zoom, tileSize);
        }

        public static double MapScale(double latitude, double zoom, int screenDpi, int tileSize)
        {
            return GroundResolution(latitude, zoom, tileSize) * screenDpi / 0.0254;
        }

        public static double MapSize(double zoom, int tileSize)
        {
            return Math.Ceiling(tileSize * Math.Pow(2, zoom));
        }

        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        public static double ToZoomLevel(double resolution)
        {
            if (resolution < 0 || resolution > 156543.03392)
                return -1;

            return System.Math.Log(156543.04 / resolution, 2);
        }

        /// <summary>
        /// Create snapshot in directory. Available extension "*.png", "*.jpeg" and "*.webp".
        /// </summary>
        public static void CreateSnapshot(IReadOnlyViewport? viewport, IEnumerable<ILayer> layers, string directory, string extension = "*.png")
        {
            SKEncodedImageFormat type;
            var ext = extension.Replace("*.", "");

            try
            {
                type = Enum.Parse<SKEncodedImageFormat>(ext[..1].ToUpper() + ext[1..].ToString());

            }
            catch (Exception)
            {
                Logger.LogDebug($"Extension {extension} not valid.");
                return;
            }

            var isAvailable = new[] { SKEncodedImageFormat.Png, SKEncodedImageFormat.Jpeg, SKEncodedImageFormat.Webp }.Contains(type);

            if (isAvailable == false)
            {
                Logger.LogDebug($"Extension {extension} not support for map snapshots.");
                return;
            }

            using var memoryStream = new Mapsui.Rendering.Skia.MapRenderer().RenderToBitmapStream(viewport, layers);

            if (memoryStream != null)
            {
                var snapshot = UniqueNameHelper.Create("Snapshot", ext);
                var path = System.IO.Path.Combine(directory, snapshot);

                using var image = SKImage.FromEncodedData(memoryStream.ToArray());
                using var data = image.Encode(type, 100);
                using var stream = System.IO.File.OpenWrite(path);

                data.SaveTo(stream);
            }
        }
    }
}
