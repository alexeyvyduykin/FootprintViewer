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

public class SettingsViewModel : DialogViewModelBase<IList<DbKeys>>
{
    private readonly IDataManager _dataManager;

    public SettingsViewModel(IReadonlyDependencyResolver dependencyResolver)
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
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            var dirtyList = new List<DbKeys>();

            foreach (var container in SourceContainers)
            {
                _ = Enum.TryParse<DbKeys>(container.Header, out var key);

                foreach (var op in container.SourceOperationsStack.Reverse())
                {
                    if (dirtyList.Contains(key) == false)
                    {
                        dirtyList.Add(key);
                    }

                    // add
                    if (op.Item2 == true)
                    {
                        _dataManager.RegisterSource(key.ToString(), op.Item1);
                    }
                    // remove
                    else if (op.Item2 == false)
                    {
                        _dataManager.UnregisterSource(key.ToString(), op.Item1);
                    }
                }
            }

            Close(DialogResultKind.Normal, dirtyList);
        });

        LanguageSettings = new LanguageSettingsViewModel(languageManager);

        LanguageSettings.Activate();
    }

    [Reactive]
    public IList<SourceContainerViewModel> SourceContainers { get; set; }

    [Reactive]
    public SourceContainerViewModel? SelectedSourceContainer { get; set; }

    [Reactive]
    public LanguageSettingsViewModel LanguageSettings { get; set; }
}
