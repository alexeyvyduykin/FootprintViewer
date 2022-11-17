using FootprintViewer.AppStates.Extensions;
using FootprintViewer.Data.DataManager;
using System.Linq;
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
    public LocalizationState LocalizationState { get; private set; } = new();

    [DataMember]
    private DataState<ISourceState> DataState { get; set; } = new();

    public void InitDefaultData(IDataManager dataManager)
    {
        var defaultSources = dataManager.GetSources();

        var dict = defaultSources
            .ToDictionary(s => s.Key, s => s.Value.Select(s => s.ToState()).ToList());

        DataState.SetExcludeValues(dict);
    }

    public void LoadData(IDataManager dataManager)
    {
        var values = DataState.GetValues();

        foreach (var (key, states) in values)
        {
            foreach (var state in states)
            {
                dataManager.RegisterSource(key, state.ToSource());
            }
        }
    }

    public void SaveData(IDataManager dataManager)
    {
        var sources = dataManager.GetSources();

        var dict = sources
            .ToDictionary(s => s.Key, s => s.Value.Select(s => s.ToState()).ToList());

        DataState.UpdateValues(dict);
    }
}
