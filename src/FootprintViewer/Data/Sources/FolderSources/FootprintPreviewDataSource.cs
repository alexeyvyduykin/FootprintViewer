using BruTile.MbTiles;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Utilities;
using SkiaSharp;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class FootprintPreviewDataSource : BaseFolderSource, IDataSource<FootprintPreview>
    {
        private readonly Random _random = new();
        private readonly DateTime _date;
        //private readonly Mapsui.Styles.Color _backgroundColorMask = new() { R = 66, G = 66, B = 66, A = 255 }; // #424242                                                                                                                                                                                                                                                                 
        private const int _previewWidth = 200;
        private const int _previewHeight = 200;

        public FootprintPreviewDataSource()
        {
            _date = DateTime.UtcNow;
        }

        private IEnumerable<FootprintPreview> GetFootprintPreviews()
        {
            if (Directory != null && SearchPattern != null)
            {
                var paths = System.IO.Directory.GetFiles(Directory, SearchPattern).Select(Path.GetFullPath);

                foreach (var path in paths)
                {
                    if (string.IsNullOrEmpty(path) == false)
                    {
                        var filename = System.IO.Path.GetFileNameWithoutExtension(path);
                        var name = filename?.Split('_').FirstOrDefault();

                        if (string.IsNullOrEmpty(filename) == false && string.IsNullOrEmpty(name) == false)
                        {
                            var tileNumber = name.Replace("-", "").ToUpper();

                            yield return new FootprintPreview()
                            {
                                Name = name,
                                Date = _date.Date.ToShortDateString(),
                                SatelliteName = $"Satellite{_random.Next(1, 6):00}",
                                SunElevation = _random.Next(0, 90),
                                CloudCoverFull = _random.Next(0, 100),
                                TileNumber = tileNumber,
                                Path = path,
                                Image = CreatePreviewImage(path),
                            };
                        }
                    }
                }
            }
        }

        public async Task<List<FootprintPreview>> GetNativeValuesAsync(IFilter<FootprintPreview>? filter)
        {
            return await Task.Run(() =>
            {
                if (filter == null)
                {
                    return GetFootprintPreviews().ToList();
                }

                return GetFootprintPreviews().Where(s => filter.Filtering(s)).ToList();
            });
        }

        private static SKImage CreatePreviewImage(string path)
        {
            var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

            var layer = new TileMemoryLayer(mbTilesTileSource);

            var extent = mbTilesTileSource.Schema.Extent;

            var viewport = new Viewport
            {
                CenterX = extent.CenterX,
                CenterY = extent.CenterY,
                Width = _previewWidth,
                Height = _previewHeight,
                Resolution = ZoomHelper.DetermineResolution(extent.Width, extent.Height, _previewWidth, _previewHeight)
            };

            var memoryStream = new Mapsui.Rendering.Skia.MapRenderer().RenderToBitmapStream(viewport, new[] { layer }/*, _backgroundColorMask*/);

            var data = memoryStream!.ToArray();

            return SKImage.FromEncodedData(data);
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<FootprintPreview, T> converter)
        {
            return await Task.Run(() =>
            {
                if (filter == null)
                {
                    return GetFootprintPreviews().Select(s => converter(s)).ToList();
                }

                return GetFootprintPreviews().Select(s => converter(s)).Where(s => filter.Filtering(s)).ToList();
            });
        }
    }
}
