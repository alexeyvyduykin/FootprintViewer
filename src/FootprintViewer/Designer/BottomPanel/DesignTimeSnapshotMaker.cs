using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeSnapshotMaker : SnapshotMaker
    {
        public DesignTimeSnapshotMaker() : base(new DesignTimeData())
        {

        }
    }
}
