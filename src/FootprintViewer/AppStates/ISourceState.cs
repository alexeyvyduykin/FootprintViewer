namespace FootprintViewer.AppStates;

public interface ISourceState : IEquatable<ISourceState>
{
    string? Key { get; set; }
}
