using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeDatabaseSourceInfo : DatabaseSourceInfo
    {
        public DesignTimeDatabaseSourceInfo() : base()
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
