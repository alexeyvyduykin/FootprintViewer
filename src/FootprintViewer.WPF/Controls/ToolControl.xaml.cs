using FootprintViewer.ViewModels;
using FootprintViewer.WPF.Controls.SidePanelTabs;
using FootprintViewer.WPF.ViewModels;
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

namespace FootprintViewer.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для ToolControl.xaml
    /// </summary>
    public partial class ToolControl : UserControl
    {
        public ToolControl()
        {
            InitializeComponent();
        }

        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsSelectorOpen == true)
            {
                MainWindow parentWindow = (MainWindow)sender;

                var w = LayerSelectorButton.ActualWidth;
                var h = LayerSelectorButton.ActualHeight;
                var H = parentWindow.ActualHeight;

                var p = LayerSelectorButton.TransformToAncestor(parentWindow).Transform(new Point(w + 10, h));

                _border.Margin = new Thickness(p.X, 0, 0, H - p.Y - h);
            }
        }

        private string _selectorName = "SelectorBorder";
        private Border _border;
        private bool _first = true;

        public bool IsSelectorOpen
        {
            get { return (bool)GetValue(IsSelectorOpenProperty); }
            set { SetValue(IsSelectorOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFilterOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectorOpenProperty =
            DependencyProperty.Register("IsSelectorOpen", typeof(bool), typeof(ToolControl), new PropertyMetadata(false));

        private void SelectorButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow parentWindow = (MainWindow)Window.GetWindow(this);

            if (parentWindow.DataContext is MainViewModel mainViewModel)
            {
                if (DataContext is ToolManager toolManager)
                {
                    if (_first == true)
                    {
                        // HACK: replace notify on event and observer
                        {
                            parentWindow.SizeChanged += ParentWindow_SizeChanged;

                            mainViewModel.WorldMapSelector.SelectedLayerObserver.Subscribe(_ =>
                            {
                                if (IsSelectorOpen == true)
                                {
                                    parentWindow.GridOverlay.Children.Remove(_border);

                                    IsSelectorOpen = false;
                                }
                            });
                        }

                        _first = false;
                    }

                    if (IsSelectorOpen == false)
                    {
                        IsSelectorOpen = true;
                      
                        var w = LayerSelectorButton.ActualWidth;
                        var h = LayerSelectorButton.ActualHeight;
                        var H = parentWindow.ActualHeight;
                        
                        var p = LayerSelectorButton.TransformToAncestor(parentWindow).Transform(new Point(w + 10, h));

                        _border = new Border()
                        {                            
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            Name = _selectorName,
                            BorderThickness = new Thickness(0),
                            Margin = new Thickness(p.X, 0, 0, H - p.Y - h),                       
                        };
                                                    
                        _border.Child = new WorldMapSelectorView() { DataContext = mainViewModel.WorldMapSelector };

                        parentWindow.GridOverlay.Children.Add(_border);                        
                    }
                    else
                    {
                        parentWindow.GridOverlay.Children.Remove(_border);

                        IsSelectorOpen = false;
                    } 
                }
            }
        }
    }
}
