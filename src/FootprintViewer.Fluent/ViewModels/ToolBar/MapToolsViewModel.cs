using FootprintViewer.UI.Services2;
using FootprintViewer.UI.ViewModels.Navigation;
using FootprintViewer.UI.ViewModels.Timelines;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.ViewModels.ToolBar;

public class MapToolsViewModel : ViewModelBase
{
    public MapToolsViewModel()
    {
        var mapService = Services.Locator.GetRequiredService<IMapService>();

        Snapshot = ReactiveCommand.CreateFromObservable<Unit, Unit>(s =>
        Observable.Start(() =>
        {
            MapHelper.CreateSnapshot(mapService.Viewport, mapService.Map.Layers, Services.MapSnapshotDir, Services.Config.SelectedMapSnapshotExtension);
        }).Delay(TimeSpan.FromSeconds(1)));

        Timelines = ReactiveCommand.CreateFromTask(TimelinesImpl);

        TimelinesOld = ReactiveCommand.CreateFromTask(TimelinesOldImpl);
    }

    public ReactiveCommand<Unit, Unit> Snapshot { get; set; }

    public ReactiveCommand<Unit, Unit> Timelines { get; }

    public ReactiveCommand<Unit, Unit> TimelinesOld { get; }

    private async Task TimelinesImpl()
    {
        var timelinesDialog = new TimelinesViewModel();

        await MainViewModel.Instance.FullScreen.NavigateDialogAsync(timelinesDialog);
    }

    private async Task TimelinesOldImpl()
    {
        var timelinesOldDialog = new TimelinesOldViewModel();

        await MainViewModel.Instance.FullScreen.NavigateDialogAsync(timelinesOldDialog);
    }
}