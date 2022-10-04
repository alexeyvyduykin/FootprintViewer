using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using DataSettingsSample.ViewModels;
using ReactiveUI;

namespace DataSettingsSample.Views
{
    public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
    {
        public SettingsView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }

        public void ClosedDialog(object sender, RoutedEventArgs args)
        {
            DialogHost.DialogHost.GetDialogSession("PrimaryDialogHost")?.Close(false);
        }
    }
}
