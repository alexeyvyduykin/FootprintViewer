﻿using Mapsui;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity
{
    public abstract class BaseInteractiveObject : IInteractiveObject
    {
        public event EventHandler? InvalidateLayer;

        protected void Invalidate()
        {
            InvalidateLayer?.Invoke(this, EventArgs.Empty);
        }

        public abstract IEnumerable<MPoint> GetActiveVertices();

        public abstract void Starting(MPoint worldPosition);

        public abstract void Moving(MPoint worldPosition);

        public abstract void Ending(MPoint worldPosition, Predicate<MPoint>? isEnd);

        public abstract void Hovering(MPoint worldPosition);
    }
}
