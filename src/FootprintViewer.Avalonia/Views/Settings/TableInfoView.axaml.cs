using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class TableInfoView : ReactiveUserControl<TableInfo>
    {
        private static readonly Dictionary<TableInfoType, string> _dict;
        private readonly IResourceDictionary _resources;

        static TableInfoView()
        {
            _dict = new()
            {
                { TableInfoType.Footprint, "footprintColumns" },
                { TableInfoType.GroundTarget, "groundTargetColumns" },
                { TableInfoType.GroundStation, "groundStationColumns" },
                { TableInfoType.Satellite, "satelliteColumns" },
                { TableInfoType.UserGeometry, "userGeometryColumns" },
            };
        }

        public TableInfoView()
        {
            InitializeComponent();

            _resources = this.Resources;

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Type, v => v.fakeItemsControl.Items, value => Convert(value, _resources)).DisposeWith(disposables);
            });
        }

        private static object Convert(TableInfoType type, IResourceDictionary resources)
        {
            if (resources.TryGetResource(_dict[type], out var res) == true)
            {
                if (res is ItemsControl itemsControl)
                {
                    return itemsControl.Items;
                }
            }

            return new List<TableColumnModel>();
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

        public void ClosedDialog(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.GetDialogSession("SecondaryDialogHost")?.Close(false);
        }

    }

    public class TableColumnModel
    {
        public string? FieldName { get; set; }

        public string? FieldType { get; set; }

        public string? Hyperlink { get; set; }

        public bool PrimaryKey { get; set; }

        public string? Description { get; set; }

        public string? Dimension { get; set; }
    }
}
