using Splat;

namespace FootprintViewer.Fluent.ViewModels;

public sealed class BottomPanel : ViewModelBase
{
    private readonly SnapshotMaker _snapshotMaker;

    public BottomPanel(IReadonlyDependencyResolver dependencyResolver)
    {
        _snapshotMaker = new SnapshotMaker(dependencyResolver);
    }

    public SnapshotMaker SnapshotMaker => _snapshotMaker;
}
