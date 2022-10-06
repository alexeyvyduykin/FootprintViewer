namespace DataSettingsSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            SettingsViewModel = new SettingsViewModel();

            FootprintList = new ListViewModel();
            GroundTargetList = new ListViewModel();
            SatelliteList = new ListViewModel();
            GroundStationList = new ListViewModel();
            UserGeometryList = new ListViewModel();
        }

        public SettingsViewModel SettingsViewModel { get; set; }

        public ListViewModel FootprintList { get; set; }

        public ListViewModel GroundTargetList { get; set; }

        public ListViewModel SatelliteList { get; set; }

        public ListViewModel GroundStationList { get; set; }

        public ListViewModel UserGeometryList { get; set; }
    }
}
