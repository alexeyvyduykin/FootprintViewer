using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeRandomSourceViewModel : RandomSourceViewModel
    {
        public DesignTimeRandomSourceViewModel() : base("RandomFootprints")
        {
            GenerateCount = 440;
        }
    }
}
