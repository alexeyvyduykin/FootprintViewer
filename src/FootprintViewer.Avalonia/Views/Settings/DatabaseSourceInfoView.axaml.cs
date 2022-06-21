using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class DatabaseSourceInfoView : ReactiveUserControl<DatabaseSourceInfo>
    {
        public DatabaseSourceInfoView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }

        public void TableInfo_Clicked(object sender, RoutedEventArgs args)
        {
            if (ViewModel != null && ViewModel.TableInfo != null)
            {
                DialogHost.DialogHost.Show(ViewModel.TableInfo, "SecondaryDialogHost");
            }
        }
    }
}
