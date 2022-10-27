﻿using DataSettingsSample.ViewModels;
using DataSettingsSample.ViewModels.Interfaces;
using System.Collections.Generic;

namespace DataSettingsSample.Designer
{
    public class DesignTimeSettingsViewModel : SettingsViewModel
    {
        public DesignTimeSettingsViewModel()
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
                },
            };
        }
    }
}