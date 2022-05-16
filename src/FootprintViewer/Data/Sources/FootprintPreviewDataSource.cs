using BruTile.MbTiles;
using FootprintViewer.FileSystem;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data.Sources
{
    public class FootprintPreviewDataSource : IFootprintPreviewDataSource
    {
        private readonly Random _random = new();
        private readonly DateTime _date;
        private readonly SolutionFolder _dataFolder;
        private readonly string _file;
        private readonly string? _subFolder;
        private readonly string? _searchPattern;
        //private readonly Mapsui.Styles.Color _backgroundColorMask = new() { R = 66, G = 66, B = 66, A = 255 }; // #424242                                                                                                                                                                                                                                                                 
        private const int _previewWidth = 200;
        private const int _previewHeight = 200;

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

        public IList<FootprintPreview> GetFootprintPreviews(IFilter<FootprintPreview>? filter)
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

            return list;
        }

        private static System.Drawing.Image CreatePreviewImage(string path)
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

            var bitmap = new Mapsui.Rendering.Skia.MapRenderer().RenderToBitmapStream(viewport, new[] { layer }/*, _backgroundColorMask*/);

            return System.Drawing.Image.FromStream(bitmap!);
        }
    }
}
