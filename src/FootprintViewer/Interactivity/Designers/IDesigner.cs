using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity.Designers
{
    public interface IDesigner : IInteractiveObject
    {
        IFeature Feature { get; }

        IList<IFeature> ExtraFeatures { get; }

        event EventHandler? BeginCreating;

        event EventHandler? Creating;

        event EventHandler? HoverCreating;

        event EventHandler? EndCreating;
    }
}
