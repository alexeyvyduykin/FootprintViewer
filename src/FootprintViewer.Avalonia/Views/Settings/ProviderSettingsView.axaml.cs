using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class ProviderSettingsView : ReactiveUserControl<ProviderSettings>
    {
        public ProviderSettingsView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                foreach (var item in ViewModel!.AvailableSources)
                {
                    item.ShowBuilderDialog.RegisterHandler(
                        async interaction =>
                        {
                            AddSourceButton.Flyout?.Hide();
                            var res = await DialogHost.DialogHost.Show(interaction.Input, "MainDialogHost");
                            var source = (res is ISourceInfo info) ? info : null;
                            interaction.SetOutput(source);
                        }).DisposeWith(disposables);
                }
            });
        }
    }
}
