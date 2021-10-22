using Mapsui;
using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer
{

    public class FeatureEventArgs : EventArgs
    {   
        public IInteractiveFeature Feature { get; set; }
    }

    public delegate void FeatureEventHandler(object sender, FeatureEventArgs e);

    // TODO: create abstract Plotter, and concrete manipulaters for each feature, Plotter fires creating events
    public class InteractiveFeatureObserver : IInteractiveFeatureObserver, IInteractiveFeatureParent
    {    
        private AddInfo? _addInfo;
        private readonly EditLayer _editLayer;
        private readonly int _minPixelsMovedForDrag = 4;

        public InteractiveFeatureObserver(EditLayer editLayer)
        {
            _editLayer = editLayer;
        }

        public event FeatureEventHandler StepCreating;

        public event FeatureEventHandler CreatingCompleted;

        public event FeatureEventHandler HoverCreating;

        public EditLayer Layer => _editLayer;

        private IInteractiveFeature CreateRectangle()
        {
            return new InteractiveRectangle(this);
        }

        private IInteractiveFeature CreateRoute()
        {
            return new InteractiveRoute(this);
        }

        private IInteractiveFeature CreateCircle()
        {
            return new InteractiveCircle(this);
        }

        private IInteractiveFeature CreatePolygon()
        {
            return new InteractivePolygon(this);
        }

        public void OnStepCreating(IInteractiveFeature feature)
        {
            StepCreating?.Invoke(this, new FeatureEventArgs() { Feature = feature });
        }

        public void OnCreatingCompleted(IInteractiveFeature feature)
        {
            CreatingCompleted?.Invoke(this, new FeatureEventArgs() { Feature = feature });
        }

        public void OnHoverCreating(IInteractiveFeature feature)
        {
            HoverCreating?.Invoke(this, new FeatureEventArgs() { Feature = feature });
        }

        // rectangle
        public (bool, BoundingBox) CreatingRectangle(Point worldPosition)
        {
            if (_addInfo == null)
            {
                var interactiveRectangle = CreateRectangle();

                _addInfo = interactiveRectangle.BeginDrawing(worldPosition);

                Layer.AddAOI(_addInfo);
                Layer.DataHasChanged();

                return (false, new BoundingBox());
            }
            else
            {
                _addInfo.Feature.EndDrawing();

                Layer.ResetAOI();
                Layer.AddAOI(_addInfo);

                BoundingBox bb = _addInfo.Feature.Geometry.BoundingBox;

                _addInfo = null;

                Layer.DataHasChanged();

                return (true, bb);
            }
        }

        public void HoverCreatingRectangle(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);

                Layer.DataHasChanged();
            }
        }

        // route
        public void CreatingRoute(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        {
            if (_addInfo == null)
            {
                var interactiveRoute = CreateRoute();

                _addInfo = interactiveRoute.BeginDrawing(worldPosition);

                Layer.AddRoute(_addInfo);
                Layer.DataHasChanged();
            }
            else
            {
                var routeGeometry = (LineString)_addInfo.Feature.Geometry;

                if (routeGeometry.Vertices.Count > 1)
                {
                    // is end?
                    foreach (var item in routeGeometry.Vertices)
                    {
                        var p = viewport.WorldToScreen(item);

                        if (IsClick(p, screenPosition) == true)
                        {
                            EndDrawingRoute();
                            return;
                        }
                    }
                }

                _addInfo.Feature.Drawing(worldPosition);

                Layer.DataHasChanged();
            }
        }

        public void HoverCreatingRoute(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);
                Layer.DataHasChanged();
            }
        }

        private void EndDrawingRoute()
        {
            if (_addInfo == null)
            {
                return;
            }

            Layer.ClearRouteHelpers();

            _addInfo.Feature.EndDrawing();

            _addInfo = null;
        }

        // polygon
        public (bool, BoundingBox) CreatingPolygon(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        {
            if (_addInfo == null)
            {
                var interactivePolygon = CreatePolygon();

                _addInfo = interactivePolygon.BeginDrawing(worldPosition);

                Layer.AddAOI(_addInfo);
                Layer.DataHasChanged();

                return (false, new BoundingBox());
            }
            else
            {
                var polygonGeometry = (LineString)_addInfo.Feature.Geometry;

                if (polygonGeometry.Vertices.Count > 2)
                {
                    // is end?

                    var p0 = viewport.WorldToScreen(polygonGeometry.Vertices[0]);

                    bool click = IsClick(p0, screenPosition);

                    if (click == true)
                    {


                        return EndDrawingPolygon();
                    }
                }

                _addInfo.Feature.Drawing(worldPosition);

                Layer.DataHasChanged();

                return (false, new BoundingBox());
            }
        }

        public void HoverCreatingPolygon(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);
                Layer.DataHasChanged();
            }
        }

        private (bool, BoundingBox) EndDrawingPolygon()
        {
            if (_addInfo == null)
            {
                return (false, new BoundingBox());
            }

            Layer.ClearAOIHelpers();

            _addInfo.Feature.EndDrawing();

            Layer.ResetAOI();
            Layer.AddAOI(_addInfo);

            var bb = _addInfo.Feature.Geometry.BoundingBox;

            _addInfo = null;

            return (true, bb);
        }

        // circle
        public (bool, BoundingBox) CreatingCircle(Point worldPosition)
        {
            if (_addInfo == null)
            {
                var interactiveCircle = CreateCircle();

                _addInfo = interactiveCircle.BeginDrawing(worldPosition);

                Layer.AddAOI(_addInfo);
                Layer.DataHasChanged();

                return (false, new BoundingBox());
            }
            else
            {
                _addInfo.Feature.EndDrawing();

                Layer.ResetAOI();
                Layer.AddAOI(_addInfo);

                var bb = _addInfo.Feature.Geometry.BoundingBox;

                _addInfo = null;

                Layer.DataHasChanged();

                return (true, bb);
            }
        }

        public void HoverCreatingCircle(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);

                Layer.DataHasChanged();
            }
        }

        private bool IsClick(Point screenPosition, Point mouseDownScreenPosition)
        {
            if (mouseDownScreenPosition == null || screenPosition == null)
            {
                return false;
            }

            return mouseDownScreenPosition.Distance(screenPosition) < _minPixelsMovedForDrag;
        }
    }
}
