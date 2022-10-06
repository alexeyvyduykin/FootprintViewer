using DataSettingsSample.ViewModels;

namespace DataSettingsSample.Designer
{
    public class DesignTimeDatabaseBuilderViewModel : DatabaseBuilderViewModel
    {
        public DesignTimeDatabaseBuilderViewModel() : base()
        {
            Host = "localhost";
            Port = 5432;
            Database = "DataSettingsSampleDatabase1";
            Username = "postgres";
            Password = "user";
        }
    }
}
