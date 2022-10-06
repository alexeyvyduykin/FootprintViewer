using DataSettingsSample.ViewModels;
using System;

namespace DataSettingsSample.Designer
{
    public class DesignTimeMainViewModel : MainWindowViewModel
    {
        private static readonly double[] values1 = new[] { 542346565.3454, 56534343.6442, 9304038592.4331, 89023437112.9033, 7882343023.6033 };
        private static readonly double[] values2 = new[] { 945877230.0542, 99723677.1293, 5093576821.9304, 39984376722.0343, 1928398435.3022 };
        private static readonly double[] values3 = new[] { 323943222.2932, 34493945.2233, 1237773343.8773, 32343478333.9823, 3223384332.0932 };
        private static readonly double[] values4 = new[] { 214343894.3439, 32299843.9843, 2938473833.0234, 34990234322.1092, 2987433544.0923 };
        private static readonly double[] values5 = new[] { 398653243.2233, 12096320.3764, 3475235478.3676, 23887122344.5744, 2345984487.2974 };

        public DesignTimeMainViewModel()
        {
            SettingsViewModel = new SettingsViewModel();

            FootprintList = new ListViewModel(values1);
            GroundTargetList = new ListViewModel(values2);
            SatelliteList = new ListViewModel(values3);
            GroundStationList = new ListViewModel(values4);
            UserGeometryList = new ListViewModel(values5);

            FootprintList.Load.Execute().Subscribe();
            GroundTargetList.Load.Execute().Subscribe();
            SatelliteList.Load.Execute().Subscribe();
            GroundStationList.Load.Execute().Subscribe();
            UserGeometryList.Load.Execute().Subscribe();
        }
    }
}
