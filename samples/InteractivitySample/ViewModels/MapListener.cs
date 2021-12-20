﻿using Mapsui.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivitySample.ViewModels
{
    public class MapListener : ReactiveObject
    {
        public MapListener()
        {

        }

        public event EventHandler? LeftClickOnMap;

        public void LeftClick(string name)
        {
            LeftClickOnMap?.Invoke(name, EventArgs.Empty);
        }

        public void LeftClick(MapInfo? mapInfo)
        {
            LeftClickOnMap?.Invoke(mapInfo, EventArgs.Empty);
        }
    }
}
