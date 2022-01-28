using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewer : GroundTargetViewer
    {
        public DesignTimeGroundTargetViewer() : base(new DesignTimeData())
        {
            var provider = new DesignDataGroundTargetProvider();
         
            UpdateAsync(provider.GetGroundTargets);
        }
    }
}
