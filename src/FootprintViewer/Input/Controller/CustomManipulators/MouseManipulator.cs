﻿using Mapsui.Geometries;

namespace FootprintViewer.Input
{
    public abstract class MouseManipulator : MapManipulator<MouseEventArgs>
    {
        protected MouseManipulator(IMapView plotView)
            : base(plotView)
        {
        }

        public Point StartPosition { get; protected set; }

        public override void Started(MouseEventArgs e)
        {
            //this.AssignAxes(e.Position);
            base.Started(e);
            StartPosition = e.Position;
        }
    }
}