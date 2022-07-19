using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeFolderSourceViewModel : FolderSourceViewModel
    {
        public DesignTimeFolderSourceViewModel() : base()
        {
            Directory = @"C:\Users\User\source\repos\AvaloniaDataPersistence\data\Provider1\";

            SearchPattern = "*.txt";
        }
    }
}
