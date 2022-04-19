using Mapsui.Nts;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity.Designers
{
    public interface IDesigner : IInteractiveObject
    {
        GeometryFeature Feature { get; }

        IList<GeometryFeature> ExtraFeatures { get; }

        event EventHandler? BeginCreating;

        event EventHandler? Creating;

        event EventHandler? HoverCreating;

        event EventHandler? EndCreating;
    }
}
