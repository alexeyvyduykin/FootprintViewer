using System;

namespace InteractiveGeometry.UI.Input.Core
{
    public abstract class InputEventArgs : EventArgs
    {
        public bool Handled { get; set; }
    }
}