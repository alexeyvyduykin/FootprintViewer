using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.Settings;
using Splat;
using System.Collections.Generic;

namespace FootprintViewer.Designer;

public class DesignTimeSettingsViewModel : SettingsViewModel
{
    private static readonly IReadonlyDependencyResolver _dependencyResolver = new DesignTimeData();

    public DesignTimeSettingsViewModel() : base(_dependencyResolver)
    {
        var _dataManager = _dependencyResolver.GetService<IDataManager>()!;

        var source = _dataManager.GetSources(DbKeys.Footprints.ToString())[0];

        SourceContainers = new List<SourceContainerViewModel>()
        {
            new SourceContainerViewModel(DbKeys.Footprints.ToString(), _dependencyResolver)
            {
                Header = DbKeys.Footprints.ToString(),
                Sources = new List<ISourceViewModel>()
                {
                    new SourceViewModel(source) { Name = "Source1" },
                    new SourceViewModel(source) { Name = "Source2" },
                    new SourceViewModel(source) { Name = "Source3" },
                },
            },
            new SourceContainerViewModel(DbKeys.GroundTargets.ToString(), _dependencyResolver)
            {
                Header = DbKeys.GroundTargets.ToString(),
                Sources = new List<ISourceViewModel>()
                {
                    new SourceViewModel(source) { Name = "Source4" },
                    new SourceViewModel(source) { Name = "Source5" },
                    new SourceViewModel(source) { Name = "Source6" },
                },
            },
            new SourceContainerViewModel(DbKeys.Satellites.ToString(), _dependencyResolver)
            {
                Header = DbKeys.Satellites.ToString(),
                Sources = new List<ISourceViewModel>()
                {
                    new SourceViewModel(source) { Name = "Source7" },
                    new SourceViewModel(source) { Name = "Source8" },
                    new SourceViewModel(source) { Name = "Source9" },
                },
            },
            new SourceContainerViewModel(DbKeys.GroundStations.ToString(), _dependencyResolver)
            {
                Header = DbKeys.GroundStations.ToString(),
                Sources = new List<ISourceViewModel>()
                {
                    new SourceViewModel(source) { Name = "Source10" },
                    new SourceViewModel(source) { Name = "Source11" },
                    new SourceViewModel(source) { Name = "Source12" },
                },
            },
            new SourceContainerViewModel(DbKeys.UserGeometries.ToString(), _dependencyResolver)
            {
                Header = DbKeys.UserGeometries.ToString(),
                Sources = new List<ISourceViewModel>()
                {
                    new SourceViewModel(source) { Name = "Source13" },
                    new SourceViewModel(source) { Name = "Source14" },
                    new SourceViewModel(source) { Name = "Source15" },
                },
            },
        };

        IsActive = true;
    }
}
