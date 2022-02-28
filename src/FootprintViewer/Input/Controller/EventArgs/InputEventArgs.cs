using System;

namespace FootprintViewer.Input
{
    public abstract class InputEventArgs : EventArgs
    {
        public bool Handled { get; set; }
    }
}