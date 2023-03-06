using FootprintViewer.Data;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using Splat;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.Timelines;

public class TimelinesViewModel : DialogViewModelBase<object>
{
    private readonly IDataManager _dataManager;

    public TimelinesViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        var cancelCommandCanExecute = this.WhenAnyValue(x => x.IsDialogOpen).ObserveOn(RxApp.MainThreadScheduler);

        NextCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Normal), cancelCommandCanExecute);
    }
}
