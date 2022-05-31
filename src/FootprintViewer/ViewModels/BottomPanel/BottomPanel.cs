using ReactiveUI;

namespace FootprintViewer.ViewModels
{
    public class BottomPanel : ReactiveObject
    {
        private readonly SnapshotMaker _snapshotMaker;

        public BottomPanel()
        {
            _snapshotMaker = new SnapshotMaker();
        }

        public SnapshotMaker SnapshotMaker => _snapshotMaker;
    }
}
