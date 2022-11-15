using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels.Settings;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.SidePanel.Tabs;

public class SettingsTabViewModel : SidePanelTabViewModel
{
    public SettingsTabViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        Options = ReactiveCommand.CreateFromTask(async () =>
        {
            var dataManager = dependencyResolver.GetExistingService<IDataManager>();
            var mainViewModel = dependencyResolver.GetExistingService<MainViewModel>();

            var settingsDialog = new SettingsViewModel(dependencyResolver);

            mainViewModel.DialogNavigationStack.To(settingsDialog);

            var dialogResult = await settingsDialog.GetDialogResultAsync();

            mainViewModel.DialogNavigationStack.Clear();

            dataManager.UpdateData();
        });

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .ToSignal()
            .InvokeCommand(Options);
    }

    private ReactiveCommand<Unit, Unit> Options { get; }
}
