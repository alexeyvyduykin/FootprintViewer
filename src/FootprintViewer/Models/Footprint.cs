﻿using BruTile;
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
    public class Footprint : ReactiveObject
    {
        public Footprint()
        {                    
        }

        public string Date { get; set; }

        public string SatelliteName { get; set; }

        public string SunElevation { get; set; }

        public string CloudCoverFull { get; set; }

        public string TileNumber { get; set; }

        public string? Path { get; set; }

        public Mapsui.Geometries.Geometry? Geometry { get; set; }

        [Reactive]
        public Image Image0 { get; set; }
    }
}
