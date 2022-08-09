using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeRandomSourceViewModel : RandomSourceViewModel
    {
        public DesignTimeRandomSourceViewModel()
        {
            Name = "RandomFootprints";
            GenerateCount = 440;
        }
    }
}
