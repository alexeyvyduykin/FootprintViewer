using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Interactivity.Decorators
{
    public class EditDecorator : BaseDecorator
    {
        private IList<MPoint> _points;
        private MPoint? _startPoint;
        private int _index;
        private MPoint _startOffsetToVertex;
        private Geometry? _startGeometry;
        private readonly bool _isRectangle;
        // HACK: without this locker Moving() passing not his order
        private bool _isEditing = false;

        public EditDecorator(GeometryFeature featureSource) : base(featureSource)
        {
            _points = featureSource.Geometry.MainVertices().Select(s=>s.ToMPoint()).ToList();

            _startOffsetToVertex = new MPoint();

            _isRectangle = IsRectangle(_points);
        }

        public override void Ending(MPoint worldPosition, Predicate<MPoint>? isEnd)
        {
            _isEditing = false;
        }

        public override IEnumerable<MPoint> GetActiveVertices() => _points;

        public override void Moving(MPoint worldPosition)
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

                    _points = geometry.MainVertices().Select(s => s.ToMPoint()).ToList();

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

                    _points = geometry.MainVertices().Select(s => s.ToMPoint()).ToList();

                    FeatureSource.Geometry = geometry;

                    FeatureSource.RenderedGeometry.Clear();
                }

                Invalidate();
            }
        }

        public override void Starting(MPoint worldPosition)
        {
            _startPoint = _points.OrderBy(v => v.Distance(worldPosition)).First();

            _index = _points.IndexOf(_startPoint);

            _startOffsetToVertex = worldPosition - _startPoint;

            _startGeometry = Copy(FeatureSource.Geometry);

            _isEditing = true;
        }

        public override void Hovering(MPoint worldPosition)
        {

        }

        private bool IsRectangle(IList<MPoint> points)
        {
            if (points.Count != 4)
            {
                return false;
            }

            return IsOrthogonal(points[0], points[1], points[2]) &&
                   IsOrthogonal(points[1], points[2], points[3]) &&
                   IsOrthogonal(points[2], points[3], points[0]);

            bool IsOrthogonal(MPoint a, MPoint b, MPoint c)
            {
                return Math.Abs((b.X - a.X) * (b.X - c.X) + (b.Y - a.Y) * (b.Y - c.Y)) < 1E-6;// == 0.0;
            }
        }
    }
}
