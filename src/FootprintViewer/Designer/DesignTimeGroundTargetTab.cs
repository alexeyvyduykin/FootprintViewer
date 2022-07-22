using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetTab : GroundTargetTab
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeGroundTargetTab() : base(_designTimeData)
        {

        }
    }
}
