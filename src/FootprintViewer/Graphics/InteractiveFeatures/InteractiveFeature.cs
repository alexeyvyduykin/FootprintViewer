using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.UI;
using System.Collections.Generic;

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
    }

    public interface IInteractiveFeature : IFeature
    {
        AddInfo BeginDrawing(Point worldPosition);

        void Drawing(Point worldPosition);

        void DrawingHover(Point worldPosition);

        void EndDrawing();

        IList<Point> EditVertices();

        bool BeginDragging(MapInfo mapInfo, double screenDistance);

        bool Dragging(Point worldPosition);

        void EndDragging();
    }
}
