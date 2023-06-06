using FootprintViewer.Data;
using PlannedScheduleOnMapSample.Models;
using PlannedScheduleOnMapSample.ViewModels;

namespace PlannedScheduleOnMapSample.Design;

public static class DesignData
{
    public static DataManager CreateDataManager()
    {
        var dataManager = new DataManager();

        dataManager.RegisterSource(MainWindowViewModel.PlannedScheduleKey, new CustomSource());

        return dataManager;
    }
}
