using System.Runtime.Serialization;

namespace FootprintViewer.AppStates
{
    [DataContract]
    public class RandomSourceState : ISourceState
    {
        [DataMember]
        public int GenerateCount { get; set; }

        [DataMember]
        public string? Name { get; set; }
    }
}
