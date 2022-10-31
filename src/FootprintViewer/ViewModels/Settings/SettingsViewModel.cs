using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.Settings
{
    public class SettingsViewModel : DialogViewModelBase<IList<DbKeys>>
    {
        private readonly IDataManager _dataManager;

        public SettingsViewModel(IDataManager dataManager)
        {
            _dataManager = dataManager;

            int counter = 0;

            var footprintsSources = dataManager.GetSources(DbKeys.Footprints.ToString());
            var groundTargetsSources = dataManager.GetSources(DbKeys.GroundTargets.ToString());
            var satellitesSources = dataManager.GetSources(DbKeys.Satellites.ToString());
            var groundStationsSources = dataManager.GetSources(DbKeys.GroundStations.ToString());
            var userGeometriesSources = dataManager.GetSources(DbKeys.UserGeometries.ToString());

            SourceContainers = new List<SourceContainerViewModel>()
            {
                new SourceContainerViewModel(DbKeys.Footprints.ToString())
                {
                    Header = DbKeys.Footprints.ToString(),
                    Sources = footprintsSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" }).ToList<ISourceViewModel>(),
                },
                new SourceContainerViewModel(DbKeys.GroundTargets.ToString())
                {
                    Header = DbKeys.GroundTargets.ToString(),
                    Sources = groundTargetsSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new SourceContainerViewModel(DbKeys.Satellites.ToString())
                {
                    Header = DbKeys.Satellites.ToString(),
                    Sources = satellitesSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new SourceContainerViewModel(DbKeys.GroundStations.ToString())
                {
                    Header = DbKeys.GroundStations.ToString(),
                    Sources = groundStationsSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new SourceContainerViewModel(DbKeys.UserGeometries.ToString())
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

                    foreach (var op in container.SourceOperationsStack)
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
        }

        [Reactive]
        public IList<SourceContainerViewModel> SourceContainers { get; set; }

        [Reactive]
        public SourceContainerViewModel? SelectedSourceContainer { get; set; }
    }
}
