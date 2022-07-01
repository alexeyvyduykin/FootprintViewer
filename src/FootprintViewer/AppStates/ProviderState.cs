using System.Collections.Generic;
using System.Runtime.Serialization;
using FootprintViewer.Data;

namespace FootprintViewer.AppStates
{
    [DataContract]
    public class ProviderState
    {
        public ProviderState()
        {
            Sources = new List<ISourceState>();
        }

        [DataMember]
        public ProviderType Type { get; init; }

        [DataMember]
        public List<ISourceState> Sources { get; private set; }
    }
}
