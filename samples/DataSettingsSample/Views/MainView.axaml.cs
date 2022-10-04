using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using DataSettingsSample.ViewModels;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.Views
{
    public partial class MainView : ReactiveUserControl<MainWindowViewModel>
    {
        public MainView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                ButtonSettings.Events()
                .Click
                .Select(s => Unit.Default)
                .InvokeCommand(this, s => s.Open)
                .DisposeWith(disposables);
            });

            Open = ReactiveCommand.CreateFromTask(Settings_Clicked);
        }

        protected ReactiveCommand<Unit, Unit> Open { get; set; }

        public async Task Settings_Clicked()
        {
            if (ViewModel != null)
            {
                await DialogHost.DialogHost.Show(ViewModel.SettingsViewModel, "PrimaryDialogHost");
            }
        }

        public void OpenedDialog(object sender, RoutedEventArgs args)
        {
            if (ViewModel != null)
            {
                DialogHost.DialogHost.Show(ViewModel.SettingsViewModel, "PrimaryDialogHost");
            }
        }
    }
}
