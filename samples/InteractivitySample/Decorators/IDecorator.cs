using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivitySample.Decorators
{
    public interface IDecorator
    {
        IEnumerable<Point> GetActiveVertices();

        void Starting(Point worldPosition);

        void Moving(Point worldPosition);

        void Ending();

        IFeature FeatureSource { get; }
    }
}
