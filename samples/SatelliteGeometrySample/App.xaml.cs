using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DatabaseCreatorSample.Data;
using FootprintViewer;
using FootprintViewer.Data;
using Mapsui;
using Mapsui.Layers;
using SatelliteGeometrySample.ViewModels;

namespace SatelliteGeometrySample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var source = DataSourceBuilder.CreateFromDatabase();
            
            var window = new MainWindow() { DataContext = CreateMainViewModel(source) };
            window.Show();
        }

        private MainViewModel CreateMainViewModel(DatabaseCreatorSample.Data.IDataSource source)
        {
            //var userDataSource = new UserDataSource();

            var map = CreateMap();

            //map.SetWorldMapLayer(userDataSource.WorldMapSources[0]/*.FirstOrDefault()*/);
                              
            var dataViewModel = new DataViewModel(map, source);

            return new MainViewModel() 
            {           
                //UserDataSource = userDataSource,
                Map = map,
                DataViewModel = dataViewModel,
            };
        }

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
                Name = nameof(LayerType.WorldMap),
            };

            return layer;
        }
    }
}
