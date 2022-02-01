using Mapsui.Geometries;
using System;

namespace FootprintViewer.InteractivityEx
{
    public class Plotter
    {
        private AddInfo? _addInfo;
        private readonly InteractiveFeature _feature;
        private bool _isEditing = false;
        private bool _isCreating = false;

        public Plotter(InteractiveFeature feature) { _feature = feature; }

        public event FeatureEventHandler? BeginCreating;

        public event FeatureEventHandler? Creating;

        public event FeatureEventHandler? EndCreating;

        public event FeatureEventHandler? Hover;

        public event EditingFeatureEventHandler? BeginEditing;

        public event EditingFeatureEventHandler? Editing;

        public event EditingFeatureEventHandler? EndEditing;

        public bool IsEditing => _isEditing;

        public bool IsCreating => _isCreating;

        public (bool, BoundingBox) CreatingFeature(Point worldPosition)
        {
            return CreatingFeature(worldPosition, point => true);
        }

        public (bool, BoundingBox) CreatingFeature(Point worldPosition, Predicate<Point> isEnd)
        {
            if (_addInfo == null)
            {
                _addInfo = _feature.BeginDrawing(worldPosition);

                _isCreating = true;

                BeginCreating?.Invoke(this, new FeatureEventArgs() { AddInfo = _addInfo });

                return (false, new BoundingBox());
            }
            else
            {
                var res = _feature.IsEndDrawing(worldPosition, isEnd);

                if (res == true)
                {
                    _addInfo.Feature.EndDrawing();

                    _isCreating = false;

                    EndCreating?.Invoke(this, new FeatureEventArgs() { AddInfo = _addInfo });

                    BoundingBox bb = _addInfo.Feature.Geometry.BoundingBox;

                    _addInfo = null;

                    return (true, bb);
                }
                else
                {
                    _addInfo.Feature.Drawing(worldPosition);

                    Creating?.Invoke(this, new FeatureEventArgs() { AddInfo = _addInfo });

                    return (false, new BoundingBox());
                }
            }
        }

        public void HoverCreatingFeature(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);

                Hover?.Invoke(this, new FeatureEventArgs() { AddInfo = _addInfo });
            }
        }

        public void EditingFeature(Point worldPosition)
        {
            if (_isEditing == true)
            {
                _feature.Editing(worldPosition);

                Editing?.Invoke(this, new EditingFeatureEventArgs() { Feature = _feature });
            }
        }

        public void BeginEditingFeature(Point worldPosition, double screenDistance)
        {
            if (_isEditing == false)
            {
                var res = _feature.BeginEditing(worldPosition, screenDistance);

                if (res == true)
                {
                    _isEditing = true;

                    BeginEditing?.Invoke(this, new EditingFeatureEventArgs() { Feature = _feature });
                }
            }
        }

        public (bool, BoundingBox) EndEditingFeature()
        {
            if (_isEditing == true)
            {
                _feature.EndEditing();

                EndEditing?.Invoke(this, new EditingFeatureEventArgs() { Feature = _feature });

                _isEditing = false;

                return (true, _feature.Geometry.BoundingBox);
            }

            return (false, new BoundingBox());
        }
    }
}
