using FootprintViewer.ViewModels.Settings;

namespace FootprintViewer.Designer
{
    public class DesignTimeRandomSourceInfo : RandomSourceInfo
    {
        public DesignTimeRandomSourceInfo() : base("RandomFootprints")
        {
            GenerateCount = 440;
        }
    }
}
