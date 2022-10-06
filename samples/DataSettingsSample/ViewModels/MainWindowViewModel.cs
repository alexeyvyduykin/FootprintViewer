using Avalonia;
using Avalonia.Platform;
using DataSettingsSample.Data;
using System;
using System.IO;
using System.Reflection;

namespace DataSettingsSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var uri1 = new Uri("avares://DataSettingsSample/Assets/File_test_01.json");
            var uri2 = new Uri("avares://DataSettingsSample/Assets/File_test_02.json");
            var path1 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_06.json"));
            var path2 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_07.json"));

            var source1 = new StreamSource<CustomJsonObject>(assets?.Open(uri1)!);
            var source2 = new StreamSource<CustomJsonObject>(assets?.Open(uri2)!);
            var source3 = new PathSource<CustomJsonObject>(path1);
            var source4 = new PathSource<CustomJsonObject>(path2);

            var repository = new Repository();

            repository.RegisterSource("footprints", source3);
            repository.RegisterSource("groundTargets", source4);

            //  repository.RegisterSource("footprints", source3);
            //  repository.RegisterSource("groundTargets", source4);

            SettingsViewModel = new SettingsViewModel();

            FootprintList = new ListViewModel("footprints", repository);
            GroundTargetList = new ListViewModel("groundTargets", repository);
            SatelliteList = new ListViewModel();
            GroundStationList = new ListViewModel();
            UserGeometryList = new ListViewModel();

            FootprintList.Load.Execute().Subscribe();
            GroundTargetList.Load.Execute().Subscribe();
        }

        public SettingsViewModel SettingsViewModel { get; set; }

        public ListViewModel FootprintList { get; set; }

        public ListViewModel GroundTargetList { get; set; }

        public ListViewModel SatelliteList { get; set; }

        public ListViewModel GroundStationList { get; set; }

        public ListViewModel UserGeometryList { get; set; }
    }
}
