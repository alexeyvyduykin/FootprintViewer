using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeDatabaseSourceViewModel : DatabaseSourceViewModel
    {
        public DesignTimeDatabaseSourceViewModel() : base()
        {
            Version = "14.1";

            Host = "localhost";

            Port = 5432;

            Database = "FootprintViewerDatabase";

            Username = "postgres";

            Password = "user";

            Table = "Footprints";
        }
    }
}
