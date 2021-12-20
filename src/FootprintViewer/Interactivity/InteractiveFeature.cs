using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity
{
    public abstract class InteractiveFeature : Feature, IInteractiveFeature
    {
        public InteractiveFeature() : base() { }

        public InteractiveFeature(IFeature feature) : base(feature) { }

        public abstract bool IsEndDrawing(Point worldPosition, Predicate<Point> isClick);

        public abstract AddInfo BeginDrawing(Point worldPosition);

        public abstract void Drawing(Point worldPosition);

        public abstract void DrawingHover(Point worldPosition);

        public abstract void EndDrawing();

        public abstract IList<Point> EditVertices();

        public abstract bool BeginEditing(Point worldPosition, double screenDistance);

        public abstract bool Editing(Point worldPosition);

        public abstract void EndEditing();
    }
}
