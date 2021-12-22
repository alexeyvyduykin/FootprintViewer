using Mapsui.Geometries;
using System;

namespace InteractivitySample.Input.Controller
{
    public class MapObserver : IMapObserver
    {
        public event StartedEventHandler? Started;

        public event DeltaEventHandler? Delta;

        public event CompletedEventHandler? Completed;

        public void OnDelta(Point worldPosition)
        {
            Delta?.Invoke(this, new DeltaEventArgs() { WorldPosition = worldPosition });
        }

        public void OnStarted(Point worldPosition, double screenDistance)
        {
            Started?.Invoke(this, new StartedEventArgs() { WorldPosition = worldPosition, ScreenDistance = screenDistance });
        }

        public void OnCompleted()
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }
    }
}
