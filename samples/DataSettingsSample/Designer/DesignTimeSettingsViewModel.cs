using DataSettingsSample.Data;
using DataSettingsSample.ViewModels;
using DataSettingsSample.ViewModels.Interfaces;
using Splat;
using System.Collections.Generic;

namespace DataSettingsSample.Designer
{
    public class DesignTimeSettingsViewModel : SettingsViewModel
    {
        private static readonly Repository _repository = new DesignTimeData().GetService<Repository>()!;

        public DesignTimeSettingsViewModel() : base(_repository)
        {
            var source = _repository.GetSources(DbKeys.Footprints.ToString())[0];

            Providers = new List<ProviderViewModel>()
            {
                new ProviderViewModel(DbKeys.Footprints)
                {
                    Header = DbKeys.Footprints.ToString(),
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel(source) { Name = "Source1" },
                        new SourceViewModel(source) { Name = "Source2" },
                        new SourceViewModel(source) { Name = "Source3" },
                    },
                },
                new ProviderViewModel(DbKeys.GroundTargets)
                {
                    Header = DbKeys.GroundTargets.ToString(),
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel(source) { Name = "Source4" },
                        new SourceViewModel(source) { Name = "Source5" },
                        new SourceViewModel(source) { Name = "Source6" },
                    },
                },
                new ProviderViewModel(DbKeys.Satellites)
                {
                    Header = DbKeys.Satellites.ToString(),
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel(source) { Name = "Source7" },
                        new SourceViewModel(source) { Name = "Source8" },
                        new SourceViewModel(source) { Name = "Source9" },
                    },
                },
                new ProviderViewModel(DbKeys.GroundStations)
                {
                    Header = DbKeys.GroundStations.ToString(),
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel(source) { Name = "Source10" },
                        new SourceViewModel(source) { Name = "Source11" },
                        new SourceViewModel(source) { Name = "Source12" },
                    },
                },
                new ProviderViewModel(DbKeys.UserGeometries)
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
        }
    }
}
