using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FootprintViewer.AppStates;

[DataContract]
public class FileSourceState : ISourceState
{
    [DataMember]
    public string? Key { get; set; }

    [DataMember]
    public IList<string> Paths { get; set; } = new List<string>();

    private bool Equals(FileSourceState? other)
    {
        return Equals(Key, other?.Key)
            && other != null
            && Paths.All(s => other.Paths.Contains(s));
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FileSourceState);
    }

    public override int GetHashCode()
        => (Key, Paths).GetHashCode();
}
