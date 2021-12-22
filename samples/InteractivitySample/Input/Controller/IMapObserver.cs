using Mapsui.Geometries;
using System;

namespace InteractivitySample.Input.Controller
{
    public class DeltaEventArgs : EventArgs
    {
        public Point WorldPosition { get; set; } = new Point();
    }

    public class StartedEventArgs : EventArgs
    {
        public Point WorldPosition { get; set; } = new Point();

        public double ScreenDistance { get; set; }
    }

    public delegate void StartedEventHandler(object sender, StartedEventArgs e);
    public delegate void DeltaEventHandler(object sender, DeltaEventArgs e);
    public delegate void CompletedEventHandler(object sender, EventArgs e);

    public interface IMapObserver
    {
        event StartedEventHandler? Started;

        event DeltaEventHandler? Delta;

        event CompletedEventHandler? Completed;

        void OnStarted(Point worldPosition, double screenDistance);

        void OnDelta(Point worldPosition);

        void OnCompleted();
    }
}
