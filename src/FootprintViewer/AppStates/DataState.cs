using FootprintViewer.AppStates.Extensions;
using FootprintViewer.Data.DataManager;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FootprintViewer.AppStates;

[DataContract]
public class DataState
{
    public void SaveDefaultSources(IReadOnlyDictionary<string, IReadOnlyList<ISource>> defaultSources)
    {
        foreach (var key in defaultSources.Keys)
        {
            if (DefaultSources.ContainsKey(key) == false)
            {
                DefaultSources.Add(key, new List<ISourceState>());
            }

            foreach (var source in defaultSources[key])
            {
                var state = source.ToState();

                var isExist = DefaultSources[key].Any(s => s.Equals(state));

                if (isExist == false)
                {
                    DefaultSources[key].Add(state);
                }
            }
        }
    }

    [DataMember]
    public Dictionary<string, IList<ISourceState>> DefaultSources { get; private set; } = new();

    [DataMember]
    public Dictionary<string, IList<ISourceState>> Sources { get; private set; } = new();
}
