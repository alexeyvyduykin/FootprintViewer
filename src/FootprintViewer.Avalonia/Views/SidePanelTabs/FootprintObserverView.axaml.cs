using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class FootprintObserverView : UserControl
    {
        public FootprintObserverView()
        {
            InitializeComponent();

           // var listBox = this.FindControl<ListBox>("listBox");

           // listBox.SelectionChanged += ListBox_SelectionChanged; //PointerPressed += ListBox_PointerPressed;
        }

        private void ListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Select item");
        }

        private void ListBox_PointerPressed(object? sender, global::Avalonia.Input.PointerPressedEventArgs e)
        {
            Debug.WriteLine("Item click");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
