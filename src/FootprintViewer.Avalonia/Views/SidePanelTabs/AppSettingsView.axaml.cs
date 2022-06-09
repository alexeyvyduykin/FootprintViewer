using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Windows.Input;
using System.IO;
using System;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class AppSettingsView : ReactiveUserControl<AppSettings>
    {
        public AppSettingsView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }

    

        public void RemoveItem_Clicked(object sender, RoutedEventArgs args)
        {
            if (sender is Button btn && btn.DataContext is ISourceInfo info)
            {
                var context = (AppSettings)this.DataContext!;

                int hghg = 0;

                //        context.RemoveFromSource1(info);
            }
        }

        public void RemoveItem1_Clicked(object sender, RoutedEventArgs args)
        {
            if (sender is Button btn && btn.DataContext is ISourceInfo info)
            {
                var context = (AppSettings)this.DataContext!;

        //        context.RemoveFromSource1(info);
            }
        }

        public void RemoveItem2_Clicked(object sender, RoutedEventArgs args)
        {
            if (sender is Button btn && btn.DataContext is ISourceInfo info)
            {
                var context = (AppSettings)this.DataContext!;

       //         context.RemoveFromSource2(info);
            }
        }

        public void RemoveItem3_Clicked(object sender, RoutedEventArgs args)
        {
            if (sender is Button btn && btn.DataContext is ISourceInfo info)
            {
                var context = (AppSettings)this.DataContext!;

       //         context.RemoveFromSource3(info);
            }
        }

        public void AddFootprintDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {    
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddFootprintRandomSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new RandomSourceInfo("RandomFootprints"), "MainDialogHost");
        }

        public void AddGroundTargetDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddGroundTargetRandomSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new RandomSourceInfo("RandomGroundTargets"), "MainDialogHost");
        }

        public void AddGroundStationDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddGroundStationRandomSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new RandomSourceInfo("RandomGroundStations"), "MainDialogHost");
        }

        public void AddSatelliteDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddSatelliteRandomSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new RandomSourceInfo("RandomSatellites"), "MainDialogHost");
        }

        public void AddUserGeometryDatabaseSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new DatabaseSourceInfo(), "MainDialogHost");
        }

        public void AddFootprintPreviewGeometryFileSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new FileSourceInfo(), "MainDialogHost");
        }

        public void AddMapBackgroundFolderSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new FolderSourceInfo(), "MainDialogHost");
        }

        public void AddFootprintPreviewFolderSource_Clicked(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.Show(new FolderSourceInfo(), "MainDialogHost");
        }
    }
}
