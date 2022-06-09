using FootprintViewer.ViewModels.Settings;

namespace FootprintViewer.Designer
{
    public class DesignTimeFolderSourceInfo : FolderSourceInfo
    {
        public DesignTimeFolderSourceInfo() : base()
        {
            Directory = @"C:\Users\User\source\repos\AvaloniaDataPersistence\data\Provider1\";

            SearchPattern = "*.txt";
        }
    }
}
