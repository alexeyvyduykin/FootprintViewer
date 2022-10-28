using DataSettingsSample.Data;
using DataSettingsSample.ViewModels.Interfaces;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.ViewModels
{
    public class SettingsViewModel : DialogViewModelBase<IList<DbKeys>>
    {
        private readonly Repository _repository;

        public SettingsViewModel(Repository repository)
        {
            _repository = repository;

            int counter = 0;

            var footprintsSources = repository.GetSources(DbKeys.Footprints.ToString());
            var groundTargetsSources = repository.GetSources(DbKeys.GroundTargets.ToString());
            var satellitesSources = repository.GetSources(DbKeys.Satellites.ToString());
            var groundStationsSources = repository.GetSources(DbKeys.GroundStations.ToString());
            var userGeometriesSources = repository.GetSources(DbKeys.UserGeometries.ToString());

            Providers = new List<ProviderViewModel>()
            {
                new ProviderViewModel(DbKeys.Footprints)
                {
                    Header = DbKeys.Footprints.ToString(),
                    Sources = footprintsSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" }).ToList<ISourceViewModel>(),
                },
                new ProviderViewModel(DbKeys.GroundTargets)
                {
                    Header = DbKeys.GroundTargets.ToString(),
                    Sources = groundTargetsSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new ProviderViewModel(DbKeys.Satellites)
                {
                    Header = DbKeys.Satellites.ToString(),
                    Sources = satellitesSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new ProviderViewModel(DbKeys.GroundStations)
                {
                    Header = DbKeys.GroundStations.ToString(),
                    Sources = groundStationsSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new ProviderViewModel(DbKeys.UserGeometries)
                {
                    Header = DbKeys.UserGeometries.ToString(),
                    Sources = userGeometriesSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
            };

            NextCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));

                var dirtyList = new List<DbKeys>();

                foreach (var provider in Providers)
                {
                    _ = Enum.TryParse<DbKeys>(provider.Header, out var key);

                    foreach (var op in provider.SourceOperationsStack)
                    {
                        if (dirtyList.Contains(key) == false)
                        {
                            dirtyList.Add(key);
                        }

                        // add
                        if (op.Item2 == true)
                        {
                            _repository.RegisterSource(key.ToString(), op.Item1);
                        }
                        // remove
                        else if (op.Item2 == false)
                        {
                            _repository.UnregisterSource(key.ToString(), op.Item1);
                        }
                    }
                }

                Close(DialogResultKind.Normal, dirtyList);
            });
        }

        [Reactive]
        public IList<ProviderViewModel> Providers { get; set; }

        [Reactive]
        public ProviderViewModel? SelectedProvider { get; set; }
    }
}
