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
                        new CustomSourceViewModel(null) { Name = "Source1" },
                        new CustomSourceViewModel(null) { Name = "Source2" },
                        new CustomSourceViewModel(null) { Name = "Source3" },
                    },
                },
                new ProviderViewModel(Data.DbKeys.GroundTargets)
                {
                    Header = "GroundTargets",
                    Sources = new List<ISourceViewModel>()
                    {
                        new CustomSourceViewModel(null) { Name = "Source4" },
                        new CustomSourceViewModel(null) { Name = "Source5" },
                        new CustomSourceViewModel(null) { Name = "Source6" },
                    },
                },
                new ProviderViewModel(Data.DbKeys.Satellites)
                {
                    Header = "Satellites",
                    Sources = new List<ISourceViewModel>()
                    {
                        new CustomSourceViewModel(null) { Name = "Source7" },
                        new CustomSourceViewModel(null) { Name = "Source8" },
                        new CustomSourceViewModel(null) { Name = "Source9" },
                    },
                },
                new ProviderViewModel(Data.DbKeys.GroundStations)
                {
                    Header = "GroundStations",
                    Sources = new List<ISourceViewModel>()
                    {
                        new CustomSourceViewModel(null) { Name = "Source10" },
                        new CustomSourceViewModel(null) { Name = "Source11" },
                        new CustomSourceViewModel(null) { Name = "Source12" },
                    },
                },
                new ProviderViewModel(Data.DbKeys.UserGeometries)
                {
                    Header = "UserGeometries",
                    Sources = new List<ISourceViewModel>()
                    {
                        new CustomSourceViewModel(null) { Name = "Source13" },
                        new CustomSourceViewModel(null) { Name = "Source14" },
                        new CustomSourceViewModel(null) { Name = "Source15" },
                    },
                },
            };
        }
    }
}
