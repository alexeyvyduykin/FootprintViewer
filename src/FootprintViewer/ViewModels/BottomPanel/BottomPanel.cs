using ReactiveUI;
using Splat;

namespace FootprintViewer.ViewModels
{
    public class BottomPanel : ReactiveObject
    {
        private readonly SnapshotMaker _snapshotMaker;

        public BottomPanel(IReadonlyDependencyResolver dependencyResolver)
        {
            _snapshotMaker = new SnapshotMaker(dependencyResolver);
        }

        public SnapshotMaker SnapshotMaker => _snapshotMaker;
    }
}
