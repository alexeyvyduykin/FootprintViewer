using FootprintViewer.Data;
using FootprintViewer.WPF;
using FootprintViewer.WPF.ViewModels;
using System.Linq;
using System.Windows;

namespace FootprintViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var map = SampleBuilder.CreateMap();

            var userDataSource = new UserDataSource();

            map.SetWorldMapLayer(userDataSource.WorldMapSources.FirstOrDefault());

            var window = new MainWindow() { DataContext = new MainViewModel(map, userDataSource) };

            window.Show();
        }
    }
}
