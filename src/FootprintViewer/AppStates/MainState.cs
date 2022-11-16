using System.Runtime.Serialization;

namespace FootprintViewer.AppStates;

[DataContract]
public class MainState
{
    [DataMember]
    public string? LastOpenDirectory { get; set; }

    [DataMember]
    public LastDatabaseState? LastOpenDatabase { get; set; }

    [DataMember]
    public DataState DataState { get; private set; } = new();

    [DataMember]
    public LocalizationState LocalizationState { get; private set; } = new();
}
