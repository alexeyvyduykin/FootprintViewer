using System;

namespace FootprintViewer
{
    public abstract class InputGesture : IEquatable<InputGesture>
    {
        public abstract bool Equals(InputGesture other);
    }
}