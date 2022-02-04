using FootprintViewer.Layers;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.UI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootprintViewer.InteractivityEx;

namespace FootprintViewer
{
    public class EditingManipulator : MouseManipulator
    {
        private bool _isEditing = false;
        private int _vertexRadius = 4;
        private EditLayer _layer = null;

        public EditingManipulator(IMapView mapView) : base(mapView) { }

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_isEditing == true)
            {
                var (isEnd, bb) = MapView.Plotter.EndEditingFeature();

                MapView.Map.PanLock = false;

                if (isEnd == true)
                {
                    MapView.NavigateToAOI(bb);
                }

                _isEditing = false;
            }

            MapView.SetCursor(CursorType.Default, "EditingManipulator.Completed");

            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);
       
            if (_isEditing == true)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

                MapView.Plotter.EditingFeature(worldPosition);

                MapView.SetCursor(CursorType.HandGrab, "EditingManipulator.Delta");
                _layer.DataHasChanged();

                e.Handled = true;
            }
            else
            {
               // MapView.SetCursor(CursorType.HandGrab, "EditingManipulator.Delta");
            }

            //e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            var screenPosition = e.Position;
            var mapInfo = MapView.GetMapInfo(screenPosition);

            _isEditing = false;

            if (mapInfo.Feature != null)
            {
                var distance = mapInfo.Resolution * _vertexRadius;

                _layer = (EditLayer)mapInfo.Layer;

                MapView.Plotter.BeginEditingFeature(mapInfo.WorldPosition, distance);

                _isEditing = true;
            }

            if (_isEditing == true)
            {
                MapView.Map.PanLock = true;
            }

            e.Handled = true;
        }
    }
    public class HoverEditingManipulator : MouseManipulator
    {
        public HoverEditingManipulator(IMapView view) : base(view) { }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);
                
            if (MapView.Plotter != null && MapView.Plotter.IsEditing == false)
            {
                var screenPosition = e.Position;
                var mapInfo = MapView.GetMapInfo(screenPosition);

                if (mapInfo.Layer != null && mapInfo.Layer is EditLayer)
                {
                    MapView.SetCursor(CursorType.Hand, "HoverEditingManipulator.Delta");
                    e.Handled = true;
                }
                else
                {
                    //MapView.SetCursor(CursorType.Default, "HoverEditingManipulator.Delta");
                }
            }
        }
    }
}
