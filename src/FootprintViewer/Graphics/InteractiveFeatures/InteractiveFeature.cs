using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.UI;
using NetTopologySuite.Operation.Distance;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{


    public abstract class InteractiveFeature : Feature, IInteractiveFeature
    {
        public InteractiveFeature() : base() { }

        public InteractiveFeature(IFeature feature) : base(feature) { }

        public abstract AddInfo BeginDrawing(Point worldPosition);

        public abstract void Drawing(Point worldPosition);

        public abstract void DrawingHover(Point worldPosition);

        public abstract void EndDrawing();

        public abstract IList<Point> EditVertices();

        public abstract bool BeginDragging(Point worldPosition, double screenDistance);

        public abstract bool Dragging(Point worldPosition);

        public abstract void EndDragging();
    }

    public interface IInteractiveFeature : IFeature
    {
        AddInfo BeginDrawing(Point worldPosition);

        void Drawing(Point worldPosition);

        void DrawingHover(Point worldPosition);

        void EndDrawing();

        IList<Point> EditVertices();

        bool BeginDragging(Point worldPosition, double screenDistance);

        bool Dragging(Point worldPosition);

        void EndDragging();
    }
}
