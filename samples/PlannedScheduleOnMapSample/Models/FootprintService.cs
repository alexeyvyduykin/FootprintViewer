using FootprintViewer.Data;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Extensions;
using Mapsui;
using Mapsui.Layers;
using PlannedScheduleOnMapSample.ViewModels;
using SpaceScience.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.Models;

public class FootprintService
{
    private readonly Map _map;
    private readonly WritableLayer _trackLayer;
    private readonly DataManager _dataManager;

    public FootprintService(Map map, DataManager dataManager)
    {
        _map = map;
        _dataManager = dataManager;

        _trackLayer = (WritableLayer)_map.Layers.FindLayer(MainWindowViewModel.TrackKey).FirstOrDefault()!;
    }

    public async Task ShowTrackAsync(string footprintName)
    {
        var res = (await _dataManager.GetDataAsync<PlannedScheduleResult>(MainWindowViewModel.PlannedScheduleKey)).FirstOrDefault()!;
        var list = res.PlannedSchedules;
        var taskResult = list.Cast<ObservationTaskResult>().Where(s => IsEquals(s.TaskName, footprintName)).FirstOrDefault()!;
        var satellite = res.Satellites.Where(s => Equals(s.Name, taskResult.SatelliteName)).FirstOrDefault()!;
        var orbit = satellite.ToOrbit();
        var node = taskResult.Node;

        var features = orbit.BuildTracks().ToFeature("");

        _trackLayer.Clear();
        _trackLayer.AddRange(features[node]);
        _trackLayer.DataHasChanged();

        static bool IsEquals(string taskName, string footprintName)
        {
            return Equals($"Footprint_{taskName}", footprintName);
        }
    }
}
