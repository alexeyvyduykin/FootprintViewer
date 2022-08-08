using System.Runtime.Serialization;

namespace FootprintViewer.AppStates
{
    [DataContract]
    public class FolderSourceState : ISourceState
    {
        [DataMember]
        public string? Name { get; set; }

        [DataMember]
        public string? Directory { get; set; }

        [DataMember]
        public string? SearchPattern { get; set; }
    }
}
