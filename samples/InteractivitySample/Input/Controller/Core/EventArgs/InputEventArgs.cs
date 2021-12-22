using System;

namespace InteractivitySample.Input.Controller.Core
{
    public abstract class InputEventArgs : EventArgs
    {
        public bool Handled { get; set; }
    }
}