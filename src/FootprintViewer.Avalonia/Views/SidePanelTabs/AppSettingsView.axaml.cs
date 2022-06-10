using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class AppSettingsView : ReactiveUserControl<AppSettings>
    {
        public AppSettingsView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }

        public void AddFootprintDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddFootprintRandomSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new RandomSourceInfo("RandomFootprints"), "MainDialogHost");
        }

        public void AddGroundTargetDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddGroundTargetRandomSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new RandomSourceInfo("RandomGroundTargets"), "MainDialogHost");
        }

        public void AddGroundStationDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddGroundStationRandomSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new RandomSourceInfo("RandomGroundStations"), "MainDialogHost");
        }

        public void AddSatelliteDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddSatelliteRandomSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new RandomSourceInfo("RandomSatellites"), "MainDialogHost");
        }

        public void AddUserGeometryDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddFootprintPreviewGeometryFileSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new FileSourceInfo(), "MainDialogHost");
        }

        public void AddMapBackgroundFolderSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new FolderSourceInfo() { SearchPattern = "*.mbtiles" }, "MainDialogHost");
        }

        public void AddFootprintPreviewFolderSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new FolderSourceInfo() { SearchPattern = "*.mbtiles" }, "MainDialogHost");
        }
    }
}
