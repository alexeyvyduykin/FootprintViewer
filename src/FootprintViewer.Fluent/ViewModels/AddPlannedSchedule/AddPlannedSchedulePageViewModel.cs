using FootprintViewer.Fluent.Helpers;
using FootprintViewer.Fluent.ViewModels.Dialogs;
using FootprintViewer.Logging;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FootprintViewer.Fluent.ViewModels.AddPlannedSchedule;

public class AddPlannedSchedulePageViewModel : DialogViewModelBase<Unit>
{
    public AddPlannedSchedulePageViewModel()
    {
        EnableCancel = true;

        CreateDemoCommand = ReactiveCommand.Create(OnCreateDemo);

        ImportCommand = ReactiveCommand.CreateFromTask(async () => await OnImportAsync());

        ConnectDatabaseCommand = ReactiveCommand.Create(OnConnectDatabase);
    }

    public override string Title { get => "Add Planned Schedule"; protected set { } }

    public ICommand CreateDemoCommand { get; }

    public ICommand ImportCommand { get; }

    public ICommand ConnectDatabaseCommand { get; }

    private void OnCreateDemo()
    {
        Navigate().To(new DemoPageViewModel());
    }

    private void OnConnectDatabase()
    {
        //Navigate().To(new WalletNamePageViewModel(WalletCreationOption.ConnectToHardwareWallet));
    }

    private async Task OnImportAsync()
    {
        try
        {
            var filePath = await FileDialogHelper.ShowOpenFileDialogAsync("Import planned schedule file", new[] { "json" });

            if (filePath is null)
            {
                return;
            }

            Navigate().To(new ImportFilePageViewModel(filePath));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }
}
