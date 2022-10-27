using DataSettingsSample.ViewModels;
using DataSettingsSample.ViewModels.Interfaces;
using System.Collections.Generic;

namespace DataSettingsSample.Designer
{
    public class DesignTimeSettingsViewModel : SettingsViewModel
    {
        public DesignTimeSettingsViewModel() : base(null)
        {
            Providers = new List<ProviderViewModel>()
            {
                new ProviderViewModel(Data.DbKeys.Footprints)
                {
                    Header = "Footprints",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel(null) { Name = "Source1" },
                        new SourceViewModel(null) { Name = "Source2" },
                        new SourceViewModel(null) { Name = "Source3" },
                    },
                },
                new ProviderViewModel(Data.DbKeys.GroundTargets)
                {
                    Header = "GroundTargets",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel(null) { Name = "Source4" },
                        new SourceViewModel(null) { Name = "Source5" },
                        new SourceViewModel(null) { Name = "Source6" },
                    },
                },
                new ProviderViewModel(Data.DbKeys.Satellites)
                {
                    Header = "Satellites",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel(null) { Name = "Source7" },
                        new SourceViewModel(null) { Name = "Source8" },
                        new SourceViewModel(null) { Name = "Source9" },
                    },
                },
                new ProviderViewModel(Data.DbKeys.GroundStations)
                {
                    Header = "GroundStations",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel(null) { Name = "Source10" },
                        new SourceViewModel(null) { Name = "Source11" },
                        new SourceViewModel(null) { Name = "Source12" },
                    },
                },
                new ProviderViewModel(Data.DbKeys.UserGeometries)
                {
                    Header = "UserGeometries",
                    Sources = new List<ISourceViewModel>()
                    {
                        new SourceViewModel(null) { Name = "Source13" },
                        new SourceViewModel(null) { Name = "Source14" },
                        new SourceViewModel(null) { Name = "Source15" },
                    },
                },
            };
        }
    }
}
