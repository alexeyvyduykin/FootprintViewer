using DataSettingsSample.ViewModels.SourceBuilders;

namespace DataSettingsSample.Designer
{
    public class DesignTimeDatabaseBuilderViewModel : DatabaseBuilderViewModel
    {
        public DesignTimeDatabaseBuilderViewModel() : base(Data.DbKeys.Footprints)
        {
            Host = "localhost";
            Port = 5432;
            Database = "DataSettingsSampleDatabase1";
            Username = "postgres";
            Password = "user";
        }
    }
}
