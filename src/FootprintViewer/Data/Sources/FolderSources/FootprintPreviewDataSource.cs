using BruTile.MbTiles;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels;
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
    public class FootprintPreviewDataSource : IDataSource<FootprintPreview>
    {
        private readonly Random _random = new();
        private readonly DateTime _date;
        private readonly string? _searchPattern;
        private readonly string? _directory;
        //private readonly Mapsui.Styles.Color _backgroundColorMask = new() { R = 66, G = 66, B = 66, A = 255 }; // #424242                                                                                                                                                                                                                                                                 
        private const int _previewWidth = 200;
        private const int _previewHeight = 200;

        public FootprintPreviewDataSource(string? directory, string? searchPattern)
        {
            _directory = directory;
            _searchPattern = searchPattern;
            _date = DateTime.UtcNow;
        }

        public async Task<List<FootprintPreview>> GetValuesAsync(IFilter<FootprintPreview>? filter = null)
        {
            return await Task.Run(() =>
            {
                var list = new List<FootprintPreview>();

                if (_directory != null && _searchPattern != null)
                {
                    var paths = Directory.GetFiles(_directory, _searchPattern).Select(Path.GetFullPath);

                    foreach (var path in paths)
                    {
                        if (string.IsNullOrEmpty(path) == false)
                        {
                            var filename = System.IO.Path.GetFileNameWithoutExtension(path);
                            var name = filename?.Split('_').FirstOrDefault();

                            if (string.IsNullOrEmpty(filename) == false && string.IsNullOrEmpty(name) == false)
                            {
                                var tileNumber = name.Replace("-", "").ToUpper();

                                var footprintPreview = new FootprintPreview(filename)
                                {
                                    Date = _date.Date.ToShortDateString(),
                                    SatelliteName = $"Satellite{_random.Next(1, 6):00}",
                                    SunElevation = _random.Next(0, 90),
                                    CloudCoverFull = _random.Next(0, 100),
                                    TileNumber = tileNumber,
                                    Path = path,
                                    Image = CreatePreviewImage(path),
                                };

                                if (filter == null || filter.Filtering(footprintPreview))
                                {
                                    list.Add(footprintPreview);
                                }
                            }
                        }
                    }
                }

                return list;
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
    }
}
