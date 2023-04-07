using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FootprintViewer.Fluent.Views;

public partial class WindowTitleBar : UserControl
{
    public WindowTitleBar()
    {
        InitializeComponent();
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        var window = (Window?)this.VisualRoot;

        window?.Close();
    }

    private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
    {
        if (this.VisualRoot is Window window)
        {
            if (window.WindowState == WindowState.Normal)
            {
                window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.WindowState = WindowState.Normal;
            }
        }
    }

    private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
    {
        if (this.VisualRoot is Window window)
        {
            window.WindowState = WindowState.Minimized;
        }
    }
}
