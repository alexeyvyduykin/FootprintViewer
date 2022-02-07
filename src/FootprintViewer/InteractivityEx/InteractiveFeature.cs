using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.InteractivityEx
{
    public abstract class InteractiveFeature : Feature, IInteractiveFeature
    {
        public InteractiveFeature(IFeature feature) : base(feature) { }

        public abstract IList<Point> EditVertices();

        public abstract bool BeginEditing(Point worldPosition, double screenDistance);

        public abstract bool Editing(Point worldPosition);

        public abstract void EndEditing();
    }
}
