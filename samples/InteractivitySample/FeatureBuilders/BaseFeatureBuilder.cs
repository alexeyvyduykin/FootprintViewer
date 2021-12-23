using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivitySample.FeatureBuilders
{
    public abstract class BaseFeatureBuilder : IFeatureBuilder
    {
        public IFeature Feature { get; protected set; } = new Feature();

        public IList<IFeature> ExtraFeatures { get; protected set; } = new List<IFeature>();

        public event EventHandler? InvalidateLayer;

        public event EventHandler? Creating;

        protected void Invalidate()
        {
            InvalidateLayer?.Invoke(this, EventArgs.Empty);
        }

        protected void CreateCallback()
        {
            Creating?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Starting(Point worldPosition);

        public abstract void Moving(Point worldPosition);

        public abstract void Ending(Point worldPosition);

        public abstract void Hover(Point worldPosition);
    }
}
