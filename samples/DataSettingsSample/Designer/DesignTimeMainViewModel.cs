using DataSettingsSample.ViewModels;

namespace DataSettingsSample.Designer
{
    public class DesignTimeMainViewModel : MainWindowViewModel
    {
        public DesignTimeMainViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
        }
    }
}
