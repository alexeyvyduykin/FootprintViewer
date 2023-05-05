using FootprintViewer.Fluent.Designer;
using Splat;

namespace FootprintViewer.Fluent.ViewModels;

public sealed partial class BottomPanel : ViewModelBase
{
    private readonly SnapshotMaker _snapshotMaker;

    public BottomPanel()
    {
        _snapshotMaker = new SnapshotMaker();
    }

    public SnapshotMaker SnapshotMaker => _snapshotMaker;
}

public partial class BottomPanel
{
    public BottomPanel(DesignDataDependencyResolver resolver)
    {
        _snapshotMaker = new SnapshotMaker(resolver);
    }
}