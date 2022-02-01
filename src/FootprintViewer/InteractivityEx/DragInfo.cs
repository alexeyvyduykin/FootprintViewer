﻿using Mapsui.Geometries;

namespace FootprintViewer.InteractivityEx
{
    public class DragInfo
    {
        public IInteractiveFeature Feature { get; set; }
        public Point Vertex { get; set; }
        public Point StartOffsetToVertex { get; set; }
    }
}