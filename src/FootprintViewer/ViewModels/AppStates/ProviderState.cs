using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels
{
    [DataContract]
    public class ProviderState
    {
        public ProviderState()
        {
            Sources = new List<ISourceInfo>();
        }

        [DataMember]
        public ProviderType Type { get; init; }

        [DataMember]
        public List<ISourceInfo> Sources { get; private set; }
    }
}
