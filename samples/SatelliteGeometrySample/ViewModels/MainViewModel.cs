using FootprintViewer.Data;
using Mapsui;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SatelliteGeometrySample.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel() { }

        [Reactive]
        public DataViewModel DataViewModel { get; set; }

        [Reactive]
        public UserDataSource UserDataSource { get; set; }

        [Reactive]
        public Map Map { get; set; }
    }
}
