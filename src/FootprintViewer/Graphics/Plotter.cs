using Mapsui;
using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Graphics
{
    public class Plotter 
    {
        private AddInfo? _addInfo;     
        private readonly List<IInteractiveFeature> _cache = new List<IInteractiveFeature>();

        public Plotter() { }

        public event FeatureEventHandler BeginCreating;

        public event FeatureEventHandler StepCreating;

        public event FeatureEventHandler EndCreating;

        public event FeatureEventHandler HoverCreating;

        public (bool, BoundingBox) CreatingConcrete(Concrete concrete, Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        {
            if (_addInfo == null)
            {
                var interactiveConcrete = concrete.CreateConcrete();

                _addInfo = interactiveConcrete.BeginDrawing(worldPosition);

                BeginCreating?.Invoke(this, new FeatureEventArgs() { Feature = _addInfo.Feature });

                return (false, new BoundingBox());
            }
            else
            {

                var res = concrete.IsEndDrawing(worldPosition, screenPosition, viewport);

                if (res == true)
                {
                    _addInfo.Feature.EndDrawing();
                 
                    BoundingBox bb = _addInfo.Feature.Geometry.BoundingBox;

                    _addInfo = null;

                    return (true, bb);
                }
                else
                {
                    _addInfo.Feature.Drawing(worldPosition);
                    
                    return (false, new BoundingBox());
                }
            }
        }

        public void HoverCreatingConcrete(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);

            }
        }
    }
}
