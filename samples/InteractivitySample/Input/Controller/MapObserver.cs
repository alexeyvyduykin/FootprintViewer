using InteractivitySample.FeatureBuilders;
using Mapsui.Geometries;
using System;

namespace InteractivitySample.Input.Controller
{
    public class MapObserver : IMapObserver
    {
        public event StartedEventHandler? Started;

        public event DeltaEventHandler? Delta;

        public event CompletedEventHandler? Completed;

        public event HoverEventHandler? Hover;
        
        public MapObserver()
        {

        }

        public MapObserver(IFeatureBuilder builder)
        {
            Started += (s, e) =>
            {
                builder.Starting(e.WorldPosition);
            };

            Delta += (s, e) =>
            {
                builder.Moving(e.WorldPosition);
            };

            Completed += (s, e) =>
            {
                builder.Ending(e.WorldPosition);
            };

            Hover += (s, e) =>
            {
                builder.Hover(e.WorldPosition);
            };
        }

        public void OnDelta(Point worldPosition)
        {
            Delta?.Invoke(this, new DeltaEventArgs() { WorldPosition = worldPosition });
        }

        public void OnStarted(Point worldPosition, double screenDistance)
        {
            Started?.Invoke(this, new StartedEventArgs() { WorldPosition = worldPosition, ScreenDistance = screenDistance });
        }

        public void OnCompleted(Point worldPosition)
        {
            Completed?.Invoke(this, new CompletedEventArgs() { WorldPosition = worldPosition });
        }

        public void OnHover(Point worldPosition)
        {
            Hover?.Invoke(this, new HoverEventArgs() { WorldPosition = worldPosition });
        }
    }
}
