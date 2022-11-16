using System.Runtime.Serialization;

namespace FootprintViewer.AppStates;

[DataContract]
public class LastDatabaseState
{
    [DataMember]
    public string? Host { get; set; }

    [DataMember]
    public string? Database { get; set; }

    [DataMember]
    public string? Username { get; set; }

    [DataMember]
    public string? Password { get; set; }
}
