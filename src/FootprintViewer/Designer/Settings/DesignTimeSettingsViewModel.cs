using FootprintViewer.ViewModels.Settings;

namespace FootprintViewer.Designer;

public class DesignTimeSettingsViewModel : SettingsViewModel
{
    public DesignTimeSettingsViewModel() : base(new DesignTimeData())
    {
        IsActive = true;
    }
}
