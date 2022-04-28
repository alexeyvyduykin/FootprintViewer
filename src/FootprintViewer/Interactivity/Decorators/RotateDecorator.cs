using Mapsui;
using Mapsui.Nts;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity.Decorators
{
    //public class RotateDecorator : BaseDecorator
    //{
    //    private readonly MPoint _center;
    //    private MPoint _rotateRight;
    //    private MPoint _startRotateRight;
    //    private MPoint _startOffsetToVertex;
    //    private Geometry? _startGeometry;
    //    private double _halfDiagonal;
    //    // HACK: without this locker Moving() passing not his order
    //    private bool _isRotating = false;

    //    public RotateDecorator(GeometryFeature featureSource) : base(featureSource)
    //    {
    //        _rotateRight = new MPoint(featureSource.Extent!.Right, featureSource.Extent.Centroid.Y);

    //        _center = featureSource.Extent.Centroid;

    //        _startRotateRight = _rotateRight;

    //        _halfDiagonal = Diagonal(featureSource.Extent) / 2.0;

    //        _startOffsetToVertex = new MPoint();
    //    }

    //    public override void Ending(MPoint worldPosition, Predicate<MPoint>? isEnd)
    //    {
    //        _rotateRight = new MPoint(FeatureSource.Extent!.Right, FeatureSource.Extent.Centroid.Y);

    //        _isRotating = false;
    //    }

    //    public override IEnumerable<MPoint> GetActiveVertices() => new[] { _rotateRight };

    //    public override void Moving(MPoint worldPosition)
    //    {
    //        if (_isRotating == true && _startGeometry != null)
    //        {
    //            var p1 = worldPosition - _startOffsetToVertex;

    //            var distance = _startRotateRight.Distance(p1);

    //            var sign = (p1 - _startRotateRight).Y >= 0 ? -1 : 1;

    //            var geometry = _startGeometry.Copy();

    //            var degrees = sign * (distance * 360.0 / _halfDiagonal);

    //            Geomorpher.Rotate(geometry, degrees, _center);

    //            _rotateRight = new MPoint(_startRotateRight.X, p1.Y);

    //            UpdateGeometry(geometry);
    //        }
    //    }

    //    public override void Starting(MPoint worldPosition)
    //    {
    //        _startRotateRight = _rotateRight;

    //        _startOffsetToVertex = worldPosition - _startRotateRight;

    //        _startGeometry = FeatureSource.Geometry!.Copy();

    //        _halfDiagonal = Diagonal(FeatureSource.Extent!) / 2.0;

    //        _isRotating = true;
    //    }

    //    public override void Hovering(MPoint worldPosition)
    //    {

    //    }

    //    private static double Diagonal(MRect box)
    //    {
    //        return Math.Sqrt(box.Width * box.Width + box.Height * box.Height);
    //    }
    //}
}
