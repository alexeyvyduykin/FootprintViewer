using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewer : GroundTargetViewer
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeGroundTargetViewer() : base(_designTimeData)
        {

        }
    }
}
