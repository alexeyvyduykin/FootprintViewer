using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Decorators
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
