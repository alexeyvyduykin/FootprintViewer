using FootprintViewer.Models;
using FootprintViewer.ViewModels;
using Mapsui.UI.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FootprintViewer.WPF.Controls.SidePanelTabs
{
    /// <summary>
    /// Логика взаимодействия для SceneSearchTab.xaml
    /// </summary>
    public partial class SceneSearchTab : UserControl
    {
        public SceneSearchTab()
        {
            InitializeComponent();
        }

        private bool _isFilterOpen = false;
        private string _filterName = "FilterBorder";
        private Border _border;

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow parentWindow = (MainWindow)Window.GetWindow(this);

            if (DataContext is SceneSearch sceneSearch)
            {
                if (_isFilterOpen == false)
                {
                    _isFilterOpen = true;

                    _border = new Border()
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Name = _filterName,
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(10),
                    };

                    _border.Child = new SceneSearchFilter() { DataContext = sceneSearch.Filter };

                    parentWindow.GridOverlay.Children.Add(_border);
                }
                else
                {                    
                    parentWindow.GridOverlay.Children.Remove(_border);

                    _isFilterOpen = false;
                }
            }
        }
    }
}
