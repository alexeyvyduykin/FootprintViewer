using FootprintViewer.Interactivity.Decorators;
using FootprintViewer.Interactivity.Designers;
using Mapsui;
using System;
using System.Linq;

namespace FootprintViewer.Interactivity
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

        public void OnDelta(MPoint worldPosition)
        {
            Delta?.Invoke(this, new DeltaEventArgs() { WorldPosition = worldPosition });
        }

        public void OnStarted(MPoint worldPosition, double screenDistance)
        {
            Started?.Invoke(this, new StartedEventArgs() { WorldPosition = worldPosition, ScreenDistance = screenDistance });
        }

        public void OnCompleted(MPoint worldPosition, Predicate<MPoint> isEnd)
        {
            Completed?.Invoke(this, new CompletedEventArgs() { WorldPosition = worldPosition, IsEnd = isEnd });
        }

        public void OnCompleted(MPoint worldPosition)
        {
            Completed?.Invoke(this, new CompletedEventArgs() { WorldPosition = worldPosition, IsEnd = null });
        }

        public void OnHover(MPoint worldPosition)
        {
            Hover?.Invoke(this, new HoverEventArgs() { WorldPosition = worldPosition });
        }
    }
}
