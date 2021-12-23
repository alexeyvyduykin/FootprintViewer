using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace InteractivitySample.FeatureBuilders
{
    public interface IFeatureBuilder
    {
        IFeature Feature { get; }

        IList<IFeature> ExtraFeatures { get; }

        event EventHandler? InvalidateLayer;

        void Starting(Point worldPosition);

        void Moving(Point worldPosition);

        void Ending(Point worldPosition);

        void Hover(Point worldPosition);
    }
}
