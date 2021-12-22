using System;

namespace InteractivitySample.Input
{
    public abstract class InputGesture : IEquatable<InputGesture>
    {
        public abstract bool Equals(InputGesture? other);
    }
}