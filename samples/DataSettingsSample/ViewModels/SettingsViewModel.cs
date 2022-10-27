using DataSettingsSample.Data;
using DataSettingsSample.ViewModels.Interfaces;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace DataSettingsSample.ViewModels
{
    public class SettingsViewModel : DialogViewModelBase<Unit>
    {
        private readonly Repository _repository;

        public SettingsViewModel(Repository repository)
        {
            _repository = repository;

            var footprintsSources = repository.GetSources("footprints");
            var groundTargetsSources = repository.GetSources("groundTargets");
            var satellitesSources = repository.GetSources("satellites");
            var groundStationsSources = repository.GetSources("groundStations");
            var userGeometriesSources = repository.GetSources("userGeometries");

            int counter = 0;

            Providers = new List<ProviderViewModel>()
            {
                new ProviderViewModel(DbKeys.Footprints)
                {
                    Header = "Footprints",
                    Sources = footprintsSources.Select(s => new CustomSourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new ProviderViewModel(DbKeys.GroundTargets)
                {
                    Header = "GroundTargets",
                    Sources = groundTargetsSources.Select(s => new CustomSourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new ProviderViewModel(DbKeys.Satellites)
                {
                    Header = "Satellites",
                    Sources = satellitesSources.Select(s => new CustomSourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new ProviderViewModel(DbKeys.GroundStations)
                {
                    Header = "GroundStations",
                    Sources = groundStationsSources.Select(s => new CustomSourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
                new ProviderViewModel(DbKeys.UserGeometries)
                {
                    Header = "UserGeometries",
                    Sources = userGeometriesSources.Select(s => new CustomSourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
                },
            };

            NextCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));

                Close(DialogResultKind.Normal, Unit.Default);
            });
        }

        [Reactive]
        public IList<ProviderViewModel> Providers { get; set; }
    }
}
