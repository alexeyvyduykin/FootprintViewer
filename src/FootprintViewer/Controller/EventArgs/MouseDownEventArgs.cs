// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OxyMouseDownEventArgs.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
namespace FootprintViewer
{
    public class MouseDownEventArgs : MouseEventArgs
    {
        public MouseButton ChangedButton { get; set; }

        public int ClickCount { get; set; }

        public HitTestResult HitTestResult { get; set; } // TODO: REMOVE THIS?
    }
}