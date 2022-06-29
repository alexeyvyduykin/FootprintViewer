using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class SourceBuilderItemsList : ReactiveUserControl<ProviderSettings>
    {
        public SourceBuilderItemsList()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                ViewModel?.Activate();

                ViewModel?.SourceBuilderItems.ForEach(s => s.BuildDialog.RegisterHandler(DoShowDialogAsync).DisposeWith(disposables));
            });
        }
        private async Task DoShowDialogAsync(InteractionContext<ISourceInfo, ISourceInfo?> interaction)
        {         
            var source = (ISourceInfo?)await DialogHost.DialogHost.Show(interaction.Input, "MainDialogHost");

            interaction.SetOutput(source);
        }
    }
}
