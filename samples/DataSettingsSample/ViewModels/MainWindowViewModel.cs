using DataSettingsSample.Data;
using DataSettingsSample.Models;
using FDataSettingsSample.Models;
using Splat;
using System;
using System.Collections.Generic;

namespace DataSettingsSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IReadonlyDependencyResolver resolver)
        {
            var repository = resolver.GetExistingService<Repository>();

            SettingsViewModel = new SettingsViewModel();

            static IEnumerable<ItemViewModel> converter(object obj)
            {
                if (obj is double value)
                {
                    yield return new ItemViewModel() { Name = $"{value}" };
                }
                else if (obj is Footprint footprint)
                {
                    yield return new ItemViewModel() { Name = $"{footprint.Value}" };
                }
                else if (obj is GroundTarget groundTarget)
                {
                    yield return new ItemViewModel() { Name = $"{groundTarget.Value}" };
                }
                else if (obj is Satellite satellite)
                {
                    yield return new ItemViewModel() { Name = $"{satellite.Value}" };
                }
                else if (obj is GroundStation groundStation)
                {
                    yield return new ItemViewModel() { Name = $"{groundStation.Value}" };
                }
                else if (obj is UserGeometry userGeometry)
                {
                    yield return new ItemViewModel() { Name = $"{userGeometry.Value}" };
                }
                else if (obj is CustomJsonObject jsonObject)
                {
                    foreach (var item in jsonObject.Values)
                    {
                        yield return new ItemViewModel() { Name = $"{item}" };
                    }
                }
            }

            Func<object, IEnumerable<ItemViewModel>> Converter1 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter2 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter3 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter4 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter5 = converter;

            FootprintList = new ListViewModel("footprints", repository, Converter1);
            GroundTargetList = new ListViewModel("groundTargets", repository, Converter2);
            SatelliteList = new ListViewModel("satellites", repository, Converter3);
            GroundStationList = new ListViewModel("groundStations", repository, Converter4);
            UserGeometryList = new ListViewModel("userGeometries", repository, Converter5);

            FootprintList.Load.Execute().Subscribe();
            GroundTargetList.Load.Execute().Subscribe();
            SatelliteList.Load.Execute().Subscribe();
            GroundStationList.Load.Execute().Subscribe();
            UserGeometryList.Load.Execute().Subscribe();
        }

        public SettingsViewModel SettingsViewModel { get; set; }

        public ListViewModel FootprintList { get; set; }

        public ListViewModel GroundTargetList { get; set; }

        public ListViewModel SatelliteList { get; set; }

        public ListViewModel GroundStationList { get; set; }

        public ListViewModel UserGeometryList { get; set; }
    }
}
