using FootprintViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для TipControl.xaml
    /// </summary>
    public partial class TipControl : ItemsControl
    {      
        public TipControl()
        {
            InitializeComponent();

            //if (DataContext != null && DataContext is Tip tip)
            //{
            //    Tip = tip;
            //    ItemsSource = new ObservableCollection<Tip>() { tip };
            //}
            //else
            //{
            //    ItemsSource = new ObservableCollection<Tip>();
            //}
        }

        public Tip? Tip { get; set; }

        public void SetPosition(double x, double y)
        {
            if (Tip != null)
            {
                Tip.X = x;
                Tip.Y = y;         
                ItemsSource = new ObservableCollection<Tip>() { Tip };
            }
        }

        public void Clear()
        {
            ItemsSource = new ObservableCollection<Tip>();
        }
    }
}
