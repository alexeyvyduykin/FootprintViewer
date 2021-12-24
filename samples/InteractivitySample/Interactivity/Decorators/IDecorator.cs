using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Decorators
{
    public interface IDecorator : IInteractiveObject
    {
        IEnumerable<Point> GetActiveVertices();

        IFeature FeatureSource { get; }
    }
}
