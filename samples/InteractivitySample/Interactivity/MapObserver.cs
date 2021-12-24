using InteractivitySample.Interactivity.Decorators;
using InteractivitySample.Interactivity.Designers;
using Mapsui.Geometries;
using System;
using System.Linq;

namespace InteractivitySample.Interactivity
{
    public class MapObserver : IMapObserver
    {
        public event StartedEventHandler? Started;

        public event DeltaEventHandler? Delta;

        public event CompletedEventHandler? Completed;

        public event HoverEventHandler? Hover;
        
        public MapObserver(IInteractiveObject interactiveObject)
        {
            if (interactiveObject is IDesigner)
            {
                Started += (s, e) =>
                {
                    interactiveObject.Starting(e.WorldPosition);
                };
            }
            else if (interactiveObject is IDecorator)
            {
                Started += (s, e) =>
                {
                    var vertices = ((IDecorator)interactiveObject).GetActiveVertices();

                    var vertexTouched = vertices.OrderBy(v => v.Distance(e.WorldPosition)).FirstOrDefault(v => v.Distance(e.WorldPosition) < e.ScreenDistance);

                    if (vertexTouched != null)
                    {
                        interactiveObject.Starting(e.WorldPosition);
                    }
                };
            }
            else
            {
                throw new Exception();
            }

            Delta += (s, e) =>
            {
                interactiveObject.Moving(e.WorldPosition);
            };

            Completed += (s, e) =>
            {
                interactiveObject.Ending(e.WorldPosition, e.IsEnd);
            };

            Hover += (s, e) =>
            {
                interactiveObject.Hovering(e.WorldPosition);
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

        public void OnCompleted(Point worldPosition, Predicate<Point> isEnd)
        {
            Completed?.Invoke(this, new CompletedEventArgs() { WorldPosition = worldPosition, IsEnd = isEnd });
        }

        public void OnCompleted(Point worldPosition)
        {
            Completed?.Invoke(this, new CompletedEventArgs() { WorldPosition = worldPosition, IsEnd = null });
        }

        public void OnHover(Point worldPosition)
        {
            Hover?.Invoke(this, new HoverEventArgs() { WorldPosition = worldPosition });
        }
    }
}
