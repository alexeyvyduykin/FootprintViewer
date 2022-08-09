using FootprintViewer.Data;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FootprintViewer.AppStates
{
    [DataContract]
    public class ProviderState
    {
        public ProviderState()
        {
            Sources = new Dictionary<string, ISourceState>();
        }

        public void Add(ISourceState state)
        {
            if (string.IsNullOrEmpty(state.Name) == true)
            {
                return;
            }

            if (Sources.ContainsKey(state.Name) == false)
            {
                Sources.Add(state.Name, state);
            }
        }

        public void Remove(ISourceState state)
        {
            if (string.IsNullOrEmpty(state.Name) == true)
            {
                return;
            }

            if (Sources.ContainsKey(state.Name) == true)
            {
                Sources.Remove(state.Name);
            }
        }

        [DataMember]
        public ProviderType Type { get; init; }

        [DataMember]
        public Dictionary<string, ISourceState> Sources { get; private set; }
    }
}
