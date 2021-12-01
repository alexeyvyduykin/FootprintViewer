using BruTile;
using BruTile.MbTiles;
using Mapsui;
using Mapsui.Rendering.Skia;
using Mapsui.Utilities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SQLite;
using System;
using System.Drawing;

namespace FootprintViewer.Models
{
    public class FootprintImage : ReactiveObject
    {
        public FootprintImage()
        {                    
        }

        public string Date { get; set; }

        public string SatelliteName { get; set; }

        public double SunElevation { get; set; }

        public double CloudCoverFull { get; set; }

        public string TileNumber { get; set; }

        public string? Path { get; set; }

        public Mapsui.Geometries.Geometry? Geometry { get; set; }

        [Reactive]
        public Image Preview { get; set; }
    }
}
