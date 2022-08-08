using System.Runtime.Serialization;

namespace FootprintViewer.AppStates
{
    [DataContract]
    public class DatabaseSourceState : ISourceState
    {
        [DataMember]
        public string? Name { get; set; }

        [DataMember]
        public string? Version { get; set; }

        [DataMember]
        public string? Host { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public string? Database { get; set; }

        [DataMember]
        public string? Username { get; set; }

        [DataMember]
        public string? Password { get; set; }

        [DataMember]
        public string? Table { get; set; }
    }
}
