﻿using Mapsui.Geometries;
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
        private readonly IInteractiveFeatureParent? _parent;

        public InteractiveFeature() : base() { }

        public InteractiveFeature(IInteractiveFeatureParent parent) : base()
        {
            _parent = parent;
        }

        public IInteractiveFeatureParent? Parent => _parent;

        public InteractiveFeature(IFeature feature) : base(feature) { }

        public abstract AddInfo BeginDrawing(Point worldPosition);

        public abstract void Drawing(Point worldPosition);

        public abstract void DrawingHover(Point worldPosition);

        public abstract void EndDrawing();

        public abstract IList<Point> EditVertices();

        public abstract bool BeginEditing(Point worldPosition, double screenDistance);

        public abstract bool Editing(Point worldPosition);

        public abstract void EndEditing();
    }

    public interface IInteractiveFeature : IFeature
    {
        IInteractiveFeatureParent? Parent { get; }

        AddInfo BeginDrawing(Point worldPosition);

        void Drawing(Point worldPosition);

        void DrawingHover(Point worldPosition);

        void EndDrawing();

        IList<Point> EditVertices();

        bool BeginEditing(Point worldPosition, double screenDistance);

        bool Editing(Point worldPosition);

        void EndEditing();
    }
}
