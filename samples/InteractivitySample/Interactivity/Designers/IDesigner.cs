using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Designers
{
    public interface IDesigner : IInteractiveObject
    {
        IFeature Feature { get; }

        IList<IFeature> ExtraFeatures { get; }
     
        event EventHandler? Creating;
    }
}
