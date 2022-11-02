using Avalonia.Controls;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class SourceBuilderItemsList : UserControl// ReactiveUserControl<ProviderViewModel>
    {
        public SourceBuilderItemsList()
        {
            InitializeComponent();

            //this.WhenActivated(disposables =>
            //{
            //    ViewModel?.Activate();

            //    ViewModel?.SourceBuilderItems.ForEach(s => s.BuildDialog.RegisterHandler(DoShowDialogAsync).DisposeWith(disposables));
            //});
        }
        private async Task DoShowDialogAsync(InteractionContext<ISourceViewModel, ISourceViewModel?> interaction)
        {
            var source = (ISourceViewModel?)await DialogHost.DialogHost.Show(interaction.Input, "MainDialogHost");

            interaction.SetOutput(source);
        }
    }
}
