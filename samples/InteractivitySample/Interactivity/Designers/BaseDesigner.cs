using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Designers
{
    public abstract class BaseDesigner : BaseInteractiveObject, IDesigner
    {
        public IFeature Feature { get; protected set; } = new Feature();

        public IList<IFeature> ExtraFeatures { get; protected set; } = new List<IFeature>();

        public event EventHandler? Creating;

        protected void CreateCallback()
        {
            Creating?.Invoke(this, EventArgs.Empty);
        }
    }
}
