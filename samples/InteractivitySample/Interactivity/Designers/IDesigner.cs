using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Designers
{
    public interface IDesigner
    {
        IFeature Feature { get; }

        IList<IFeature> ExtraFeatures { get; }

        event EventHandler? InvalidateLayer;

        event EventHandler? Creating;

        void Starting(Point worldPosition);

        void Moving(Point worldPosition);

        void Ending(Point worldPosition, Predicate<Point>? isEnd);

        void Hover(Point worldPosition);
    }
}
