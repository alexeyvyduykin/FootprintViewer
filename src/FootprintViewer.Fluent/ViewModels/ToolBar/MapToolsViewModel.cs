using FootprintViewer.Fluent.Designer;
using FootprintViewer.Fluent.ViewModels.Navigation;
using FootprintViewer.Fluent.ViewModels.Timelines;
using Mapsui;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.ToolBar;

public partial class MapToolsViewModel : ViewModelBase
{
    public MapToolsViewModel()
    {
        var map = Services.Locator.GetRequiredService<Map>();
        var mapNavigator = Services.Locator.GetRequiredService<MapNavigator>();

        Snapshot = ReactiveCommand.CreateFromObservable<Unit, Unit>(s =>
        Observable.Start(() =>
        {
            MapHelper.CreateSnapshot(mapNavigator.Viewport, map.Layers, Services.MapSnapshotDir, Services.Config.SelectedMapSnapshotExtension);
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

public partial class MapToolsViewModel
{
    public MapToolsViewModel(DesignDataDependencyResolver _)
    {
        Snapshot = ReactiveCommand.CreateFromObservable<Unit, Unit>(s => Observable.Start(() => { }).Delay(TimeSpan.FromSeconds(1)));
        Timelines = ReactiveCommand.Create(() => { });
        TimelinesOld = ReactiveCommand.Create(() => { });
    }
}