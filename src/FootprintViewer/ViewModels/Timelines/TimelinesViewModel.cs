using FootprintViewer.Data;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using Splat;

namespace FootprintViewer.ViewModels.Timelines;

public class TimelinesViewModel : DialogViewModelBase<object>
{
    private readonly IDataManager _dataManager;

    public TimelinesViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        NextCommand = ReactiveCommand.Create(() =>
        {
            Close(DialogResultKind.Normal);
        });
    }
}
