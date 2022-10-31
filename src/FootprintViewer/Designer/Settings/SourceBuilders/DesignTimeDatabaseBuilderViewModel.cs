using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels.Settings.SourceBuilders;

namespace FootprintViewer.Designer;

public class DesignTimeDatabaseBuilderViewModel : DatabaseBuilderViewModel
{
    public DesignTimeDatabaseBuilderViewModel() : base(DbKeys.Footprints.ToString())
    {
        Host = "localhost";
        Port = 5432;
        Database = "DataSettingsSampleDatabase1";
        Username = "postgres";
        Password = "user";
    }
}
