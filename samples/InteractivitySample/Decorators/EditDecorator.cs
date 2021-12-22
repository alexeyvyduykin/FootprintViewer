using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InteractivitySample.Decorators
{
    public class EditDecorator : BaseDecorator
    {
        private IList<Point> _points;
        private Point? _startPoint;
        private int _index;
        private Point _startOffsetToVertex;
        private IGeometry? _startGeometry;
        private readonly bool _isRectangle;
        // HACK: without this locker Moving() passing not his order
        private bool _isEditing = false;

        public EditDecorator(IFeature featureSource) : base(featureSource)
        {
            _points = featureSource.Geometry.MainVertices();

            _startOffsetToVertex = new Point();

            _isRectangle = IsRectangle(_points);
        }

        public override void Ending()
        {
            _isEditing = false;
        }

        public override IEnumerable<Point> GetActiveVertices() => _points;

        public override void Moving(Point worldPosition)
        {
            if (_isEditing == true && _startGeometry != null)
            {
                if (_isRectangle == false)
                {
                    var p1 = worldPosition - _startOffsetToVertex;

                    var delta = p1 - _startPoint;

                    var geometry = Copy(_startGeometry);

                    var pp = geometry.MainVertices()[_index];

                    Geomorpher.Translate(pp, delta.X, delta.Y);

                    _points = geometry.MainVertices();

                    FeatureSource.Geometry = geometry;

                    FeatureSource.RenderedGeometry.Clear();
                }
                else
                {
                    var p1 = worldPosition - _startOffsetToVertex;

                    var delta = p1 - _startPoint;

                    var geometry = Copy(_startGeometry);

                    var prev = (_index - 1) < 0 ? 3 : _index - 1;
                    var next = (_index + 1) > 3 ? 0 : _index + 1;

                    var pp = geometry.MainVertices()[_index];
                    var pPrev = geometry.MainVertices()[prev];
                    var pNext = geometry.MainVertices()[next];

                    bool isVertical = Math.Abs(pp.X - pPrev.X) < 1E-4 ? true : false;

                    Geomorpher.Translate(pp, delta.X, delta.Y);

                    if (isVertical) // vertical
                    {
                        Geomorpher.Translate(pPrev, delta.X, 0.0);
                        Geomorpher.Translate(pNext, 0.0, delta.Y);
                    }
                    else // horizontal
                    {
                        Geomorpher.Translate(pPrev, 0.0, delta.Y);
                        Geomorpher.Translate(pNext, delta.X, 0.0);
                    }

                    _points = geometry.MainVertices();

                    FeatureSource.Geometry = geometry;

                    FeatureSource.RenderedGeometry.Clear();
                }
            }
        }

        public override void Starting(Point worldPosition)
        {
            _startPoint = _points.OrderBy(v => v.Distance(worldPosition)).First();

            _index = _points.IndexOf(_startPoint);

            _startOffsetToVertex = worldPosition - _startPoint;

            _startGeometry = Copy(FeatureSource.Geometry);

            _isEditing = true;
        }

        private bool IsRectangle(IList<Point> points)
        {
            if (points.Count != 4)
            {
                return false;
            }

            return IsOrthogonal(points[0], points[1], points[2]) &&
                   IsOrthogonal(points[1], points[2], points[3]) &&
                   IsOrthogonal(points[2], points[3], points[0]);

            bool IsOrthogonal(Point a, Point b, Point c)
            {
                return Math.Abs((b.X - a.X) * (b.X - c.X) + (b.Y - a.Y) * (b.Y - c.Y)) < 1E-6;// == 0.0;
            }
        }
    }
}
