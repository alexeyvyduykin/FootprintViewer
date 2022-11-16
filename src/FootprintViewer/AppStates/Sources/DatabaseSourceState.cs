using System.Runtime.Serialization;

namespace FootprintViewer.AppStates;

[DataContract]
public class DatabaseSourceState : ISourceState
{
    [DataMember]
    public string? Key { get; set; }

    [DataMember]
    public string? ConnectionString { get; set; }

    [DataMember]
    public string? TableName { get; set; }

    [DataMember]
    public bool IsEditable { get; set; }

    private bool Equals(DatabaseSourceState? other)
    {
        return Equals(Key, other?.Key)
            && Equals(ConnectionString, other?.ConnectionString)
            && Equals(TableName, other?.TableName)
            && Equals(IsEditable, other?.IsEditable);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as DatabaseSourceState);
    }

    public override int GetHashCode()
        => (Key, ConnectionString, IsEditable).GetHashCode();
}
