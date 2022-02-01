﻿using Mapsui.Geometries;
using System;

namespace FootprintViewer.Interactivity
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

    public class CompletedEventArgs : EventArgs
    {
        public Point WorldPosition { get; set; } = new Point();

        public Predicate<Point>? IsEnd { get; set; }
    }

    public class HoverEventArgs : EventArgs
    {
        public Point WorldPosition { get; set; } = new Point();
    }

    public delegate void StartedEventHandler(object sender, StartedEventArgs e);
    public delegate void DeltaEventHandler(object sender, DeltaEventArgs e);
    public delegate void CompletedEventHandler(object sender, CompletedEventArgs e);
    public delegate void HoverEventHandler(object sender, HoverEventArgs e);

    public interface IMapObserver
    {
        event StartedEventHandler? Started;

        event DeltaEventHandler? Delta;

        event CompletedEventHandler? Completed;

        event HoverEventHandler? Hover;

        void OnStarted(Point worldPosition, double screenDistance);

        void OnDelta(Point worldPosition);

        void OnCompleted(Point worldPosition, Predicate<Point> isEnd);

        void OnCompleted(Point worldPosition);

        void OnHover(Point worldPosition);
    }
}
