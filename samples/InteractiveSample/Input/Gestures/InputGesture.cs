using System;

namespace InteractiveSample.Input
{
    public abstract class InputGesture : IEquatable<InputGesture>
    {
        public abstract bool Equals(InputGesture? other);
    }
}