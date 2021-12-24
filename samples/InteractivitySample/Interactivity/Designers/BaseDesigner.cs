using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Designers
{
    public abstract class BaseDesigner : BaseInteractiveObject, IDesigner
    {
        public IFeature Feature { get; protected set; } = new Feature();

        public IList<IFeature> ExtraFeatures { get; protected set; } = new List<IFeature>();

        public event EventHandler? BeginCreating;

        public event EventHandler? Creating;

        public event EventHandler? HoverCreating;

        public event EventHandler? EndCreating;

        protected void BeginCreatingCallback()
        {
            BeginCreating?.Invoke(this, EventArgs.Empty);
        }

        protected void CreatingCallback()
        {
            Creating?.Invoke(this, EventArgs.Empty);
        }

        protected void HoverCreatingCallback()
        {
            HoverCreating?.Invoke(this, EventArgs.Empty);
        }

        protected void EndCreatingCallback()
        {
            EndCreating?.Invoke(this, EventArgs.Empty);
        }
    }
}
