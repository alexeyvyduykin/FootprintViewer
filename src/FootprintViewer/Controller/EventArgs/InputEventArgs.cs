using System;

namespace FootprintViewer
{
    public abstract class InputEventArgs : EventArgs
    {
        public bool Handled { get; set; }
    }
}