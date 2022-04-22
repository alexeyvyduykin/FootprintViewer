using System;

namespace InteractiveGeometry.UI.Input
{
    public abstract class InputGesture : IEquatable<InputGesture>
    {
        public abstract bool Equals(InputGesture? other);
    }
}