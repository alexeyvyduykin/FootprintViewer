using System;

namespace InteractiveSample.Input.Controller.Core
{
    public abstract class InputEventArgs : EventArgs
    {
        public bool Handled { get; set; }
    }
}