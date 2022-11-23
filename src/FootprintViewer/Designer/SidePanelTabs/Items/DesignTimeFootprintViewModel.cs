﻿using FootprintViewer.Data;
using FootprintViewer.ViewModels.SidePanel.Items;
using NetTopologySuite.Geometries;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintViewModel : FootprintViewModel
    {
        private readonly static Random _random = new();

        public DesignTimeFootprintViewModel() : base(BuildModel())
        {
            IsShowInfo = true;
        }

        public static Footprint BuildModel()
        {
            return new Footprint()
            {
                Name = $"Footprint{_random.Next(1, 101):000}",
                SatelliteName = $"Satellite{_random.Next(1, 10):00}",
                Center = new Point(_random.Next(-180, 180), _random.Next(-90, 90)),
                Begin = DateTime.Now,
                Duration = _random.Next(20, 40),
                Node = _random.Next(1, 16),
                Direction = (SatelliteStripDirection)_random.Next(0, 2),
            };
        }
    }
}
