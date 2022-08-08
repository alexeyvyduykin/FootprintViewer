using System.Runtime.Serialization;

namespace FootprintViewer.AppStates
{
    [DataContract]
    public class FileSourceState : ISourceState
    {
        [DataMember]
        public string? Name { get; set; }

        [DataMember]
        public string? Path { get; set; }

        [DataMember]
        public string? FilterName { get; set; }

        [DataMember]
        public string? FilterExtension { get; set; }
    }
}
