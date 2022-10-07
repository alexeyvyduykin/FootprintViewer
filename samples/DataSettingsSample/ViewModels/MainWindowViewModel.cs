using DataSettingsSample.Data;
using Splat;
using System;

namespace DataSettingsSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IReadonlyDependencyResolver resolver)
        {
            var repository = resolver.GetExistingService<Repository>();

            SettingsViewModel = new SettingsViewModel();

            FootprintList = new ListViewModel("footprints", repository);
            GroundTargetList = new ListViewModel("groundTargets", repository);
            SatelliteList = new ListViewModel("satellites", repository);
            GroundStationList = new ListViewModel("groundStations", repository);
            UserGeometryList = new ListViewModel("userGeometries", repository);

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
