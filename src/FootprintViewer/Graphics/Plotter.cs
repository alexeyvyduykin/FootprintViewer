using Mapsui;
using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Graphics
{
    public class FeatureEventArgs : EventArgs
    {
        public AddInfo AddInfo { get; set; }
    }

    public delegate void FeatureEventHandler(object sender, FeatureEventArgs e);

    public class Plotter 
    {
        private AddInfo? _addInfo;             
        private readonly InteractiveFeature _feature;

        public Plotter(InteractiveFeature feature) { _feature = feature; }

        public event FeatureEventHandler? BeginCreating;

        public event FeatureEventHandler? Creating;

        public event FeatureEventHandler? EndCreating;

        public event FeatureEventHandler? Hover;

        public (bool, BoundingBox) CreatingConcrete(Point worldPosition)
        {
            return CreatingConcrete(worldPosition, point => true);
        }

        public (bool, BoundingBox) CreatingConcrete(Point worldPosition, Predicate<Point> isEnd)
        {
            if (_addInfo == null)
            {          
                _addInfo = _feature.BeginDrawing(worldPosition);

                BeginCreating?.Invoke(this, new FeatureEventArgs() { AddInfo = _addInfo });

                return (false, new BoundingBox());
            }
            else
            {
                var res = _feature.IsEndDrawing(worldPosition, isEnd);

                if (res == true)
                {
                    _addInfo.Feature.EndDrawing();

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

        public void HoverCreatingConcrete(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);

                Hover?.Invoke(this, new FeatureEventArgs() { AddInfo = _addInfo });
            }
        }
    }
}
