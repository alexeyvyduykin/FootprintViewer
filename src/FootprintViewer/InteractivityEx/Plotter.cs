using Mapsui.Geometries;
using System;

namespace FootprintViewer.InteractivityEx
{
    public class Plotter
    {
        private readonly InteractiveFeature _feature;
        private bool _isEditing = false;

        public Plotter(InteractiveFeature feature) { _feature = feature; }

        public event EditingFeatureEventHandler? BeginEditing;

        public event EditingFeatureEventHandler? Editing;

        public event EditingFeatureEventHandler? EndEditing;

        public bool IsEditing => _isEditing;

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
