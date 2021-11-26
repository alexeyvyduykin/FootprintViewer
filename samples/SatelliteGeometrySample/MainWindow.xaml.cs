using FootprintViewer;
using FootprintViewer.Data;
using Mapsui;
using SatelliteGeometrySample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SatelliteGeometrySample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
                    
            MapControl.Info += MapControl_Info;                            
        }

        private void MapControl_Info(object sender, Mapsui.UI.MapInfoEventArgs e)
        {            
            if (DataContext != null && DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.DataViewModel.UpdateInfo(e);
            }
        }
    }
}
