namespace DataSettingsSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
        }

        public SettingsViewModel SettingsViewModel { get; set; }
    }
}
