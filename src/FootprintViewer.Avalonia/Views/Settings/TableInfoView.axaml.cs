using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class TableInfoView : UserControl
    {
        public TableInfoView()
        {
            InitializeComponent();
        }

        private async void HandleLinkClick(object sender, RoutedEventArgs e)
        {
            await OpenUrl(@"http://postgis.net");
            e.Handled = true;
        }

        private static async Task OpenUrl(string url)
        {
            await Task.Run(() =>
            {
                try
                {
                    Process.Start(url);
                }
                catch
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
            });
        }
    }
}
