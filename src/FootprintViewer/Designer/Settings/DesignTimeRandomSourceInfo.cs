using FootprintViewer.ViewModels;

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
