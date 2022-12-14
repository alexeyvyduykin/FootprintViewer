using FootprintViewer.AppStates;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.Settings;

public class ConnectionViewModel : DialogViewModelBase<object>
{
    private readonly IDataManager _dataManager;

    public ConnectionViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        var languageManager = dependencyResolver.GetExistingService<ILanguageManager>();

        int counter = 0;

        var footprintsSources = _dataManager.GetSources(DbKeys.Footprints.ToString());
        var groundTargetsSources = _dataManager.GetSources(DbKeys.GroundTargets.ToString());
        var satellitesSources = _dataManager.GetSources(DbKeys.Satellites.ToString());
        var groundStationsSources = _dataManager.GetSources(DbKeys.GroundStations.ToString());
        var userGeometriesSources = _dataManager.GetSources(DbKeys.UserGeometries.ToString());

        SourceContainers = new List<SourceContainerViewModel>()
        {
            new SourceContainerViewModel(DbKeys.Footprints.ToString(), dependencyResolver)
            {
                Header = DbKeys.Footprints.ToString(),
                Sources = footprintsSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" }).ToList<ISourceViewModel>(),
            },
            new SourceContainerViewModel(DbKeys.GroundTargets.ToString(), dependencyResolver)
            {
                Header = DbKeys.GroundTargets.ToString(),
                Sources = groundTargetsSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
            },
            new SourceContainerViewModel(DbKeys.Satellites.ToString(), dependencyResolver)
            {
                Header = DbKeys.Satellites.ToString(),
                Sources = satellitesSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
            },
            new SourceContainerViewModel(DbKeys.GroundStations.ToString(), dependencyResolver)
            {
                Header = DbKeys.GroundStations.ToString(),
                Sources = groundStationsSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
            },
            new SourceContainerViewModel(DbKeys.UserGeometries.ToString(), dependencyResolver)
            {
                Header = DbKeys.UserGeometries.ToString(),
                Sources = userGeometriesSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
            },
        };

        NextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainState = dependencyResolver.GetExistingService<MainState>();

            await Task.Delay(TimeSpan.FromSeconds(0.1));

            foreach (var container in SourceContainers)
            {
                _ = Enum.TryParse<DbKeys>(container.Header, out var key);

                foreach (var (source, op) in container.SourceOperationsStack.Reverse())
                {
                    // add
                    if (op == true)
                    {
                        _dataManager.RegisterSource(key.ToString(), source);
                    }
                    // remove
                    else if (op == false)
                    {
                        _dataManager.UnregisterSource(key.ToString(), source);
                    }
                }
            }

            mainState.SaveData(_dataManager);

            Close(DialogResultKind.Normal);
        });
    }

    [Reactive]
    public IList<SourceContainerViewModel> SourceContainers { get; set; }

    [Reactive]
    public SourceContainerViewModel? SelectedSourceContainer { get; set; }
}
