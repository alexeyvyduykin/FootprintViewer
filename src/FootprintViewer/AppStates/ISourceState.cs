using System;

namespace FootprintViewer.AppStates;

public interface ISourceState : IEquatable<object?>
{
    string? Key { get; set; }
}
