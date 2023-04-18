using Splat;

namespace FootprintViewer.Fluent.ViewModels;

public sealed class BottomPanel : ViewModelBase
{
    private readonly SnapshotMaker _snapshotMaker;

    public BottomPanel()
    {
        _snapshotMaker = new SnapshotMaker();
    }

    public SnapshotMaker SnapshotMaker => _snapshotMaker;
}
