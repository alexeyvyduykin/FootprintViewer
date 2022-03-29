using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity.Designers
{
    public class PointDesigner : BaseDesigner
    {
        private bool _skip;
        private int _counter;

        public override IEnumerable<Point> GetActiveVertices()
        {
            if (Feature.Geometry != null)
            {
                return Feature.Geometry.MainVertices();
            }

            return new Point[] { };
        }

        public override void Starting(Point worldPosition)
        {
            _skip = false;
            _counter = 0;
        }

        public override void Moving(Point worldPosition)
        {
            if (_counter++ > 0)
            {
                _skip = true;
            }
        }

        public override void Ending(Point worldPosition, Predicate<Point>? isEnd)
        {
            if (_skip == false)
            {
                CreatingFeature(worldPosition);
            }
        }

        public override void Hovering(Point worldPosition)
        {

        }

        public void CreatingFeature(Point worldPosition)
        {
            EndDrawing(worldPosition);

            EndCreatingCallback();

            return;
        }

        public void EndDrawing(Point worldPosition)
        {
            var geometry = new Point(worldPosition.X, worldPosition.Y);

            Feature = new Feature() { Geometry = geometry };
        }
    }
}
