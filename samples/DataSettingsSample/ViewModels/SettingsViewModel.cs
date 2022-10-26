using DataSettingsSample.ViewModels.Interfaces;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace DataSettingsSample.ViewModels
{
    public class SettingsViewModel : DialogViewModelBase<Unit>
    {
        public SettingsViewModel()
        {
            Providers = new List<ProviderViewModel>()
            {
                new ProviderViewModel()
                {
                    Header = "Footprints",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel() { Name = "Source1" },
                        new SourceViewModel() { Name = "Source2" },
                        new SourceViewModel() { Name = "Source3" },
                    },
                    SourceBuilderItems = new List<SourceBuilderItemViewModel>
                    {
                        new SourceBuilderItemViewModel(){ Name = ".database" },
                        new SourceBuilderItemViewModel(){ Name = ".json" },
                    }
                },
                new ProviderViewModel()
                {
                    Header = "GroundTargets",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel() { Name = "Source4" },
                        new SourceViewModel() { Name = "Source5" },
                        new SourceViewModel() { Name = "Source6" },
                    },
                    SourceBuilderItems = new List<SourceBuilderItemViewModel>
                    {
                        new SourceBuilderItemViewModel(){ Name = ".database" },
                        new SourceBuilderItemViewModel(){ Name = ".json" },
                    }
                },
                new ProviderViewModel()
                {
                    Header = "Satellites",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel() { Name = "Source7" },
                        new SourceViewModel() { Name = "Source8" },
                        new SourceViewModel() { Name = "Source9" },
                    },
                    SourceBuilderItems = new List<SourceBuilderItemViewModel>
                    {
                        new SourceBuilderItemViewModel(){ Name = ".database" },
                        new SourceBuilderItemViewModel(){ Name = ".json" },
                    }
                },
                new ProviderViewModel()
                {
                    Header = "GroundStations",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel() { Name = "Source10" },
                        new SourceViewModel() { Name = "Source11" },
                        new SourceViewModel() { Name = "Source12" },
                    },
                    SourceBuilderItems = new List<SourceBuilderItemViewModel>
                    {
                        new SourceBuilderItemViewModel(){ Name = ".database" },
                        new SourceBuilderItemViewModel(){ Name = ".json" },
                    }
                },
                new ProviderViewModel()
                {
                    Header = "UserGeometries",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel() { Name = "Source13" },
                        new SourceViewModel() { Name = "Source14" },
                        new SourceViewModel() { Name = "Source15" },
                    },
                    SourceBuilderItems = new List<SourceBuilderItemViewModel>
                    {
                        new SourceBuilderItemViewModel(){ Name = ".database" },
                        new SourceBuilderItemViewModel(){ Name = ".json" },
                    }
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
