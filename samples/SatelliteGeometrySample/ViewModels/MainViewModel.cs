using FootprintViewer;
using FootprintViewer.Data;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SatelliteGeometrySample.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            DataSource = new DataSource();

            Map = CreateMap();

            Map.SetWorldMapLayer(DataSource.WorldMapSources[0]/*.FirstOrDefault()*/);

            DataViewModel = new DataViewModel(Map);


        }

        [Reactive]
        public DataViewModel DataViewModel { get; set; }

        public static Map CreateMap()
        {
            var map = new Map()
            {
                CRS = "EPSG:3857",
                // Transformation = new MinimalTransformation(),
            };

            map.Layers.Add(CreateEmptyBackgroundLayer()); // BackgroundLayer

            //map.Home = (n) => n.NavigateTo(editLayer.Envelope.Grow(editLayer.Envelope.Width * 0.2));

            return map;
        }

        private static ILayer CreateEmptyBackgroundLayer()
        {
            var layer = new Layer()
            {
                Name = nameof(LayerType.BackgroundLayer),
            };

            return layer;
        }

        [Reactive]
        public DataSource DataSource { get; set; }


        [Reactive]
        public Map Map { get; set; }
    }
}
