using Mapsui;
using System;

namespace InteractiveGeometry
{
    public class DeltaEventArgs : EventArgs
    {
        public MPoint WorldPosition { get; set; } = new MPoint();
    }

    public class StartedEventArgs : EventArgs
    {
        public MPoint WorldPosition { get; set; } = new MPoint();

        public double ScreenDistance { get; set; }
    }

    public class CompletedEventArgs : EventArgs
    {
        public MPoint WorldPosition { get; set; } = new MPoint();

        public Predicate<MPoint>? IsEnd { get; set; }
    }

    public class HoverEventArgs : EventArgs
    {
        public MPoint WorldPosition { get; set; } = new MPoint();
    }

    public delegate void StartedEventHandler(object sender, StartedEventArgs e);
    public delegate void DeltaEventHandler(object sender, DeltaEventArgs e);
    public delegate void CompletedEventHandler(object sender, CompletedEventArgs e);
    public delegate void HoverEventHandler(object sender, HoverEventArgs e);

    public interface IInteractiveBehavior
    {
        event StartedEventHandler? Started;

        event DeltaEventHandler? Delta;

        event CompletedEventHandler? Completed;

        event HoverEventHandler? Hover;

        void OnStarted(MPoint worldPosition, double screenDistance);

        void OnDelta(MPoint worldPosition);

        void OnCompleted(MPoint worldPosition, Predicate<MPoint> isEnd);

        void OnCompleted(MPoint worldPosition);

        void OnHover(MPoint worldPosition);
    }
}
