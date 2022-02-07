using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.InteractivityEx
{
    public interface IInteractiveFeature : IFeature
    {
        IList<Point> EditVertices();

        bool BeginEditing(Point worldPosition, double screenDistance);

        bool Editing(Point worldPosition);

        void EndEditing();
    }
}
