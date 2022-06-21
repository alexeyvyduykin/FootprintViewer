namespace FootprintViewer.ViewModels
{
    public enum TableInfoType { Footprint, GroundTarget, Satellite, GroundStation, UserGeometry };

    public class TableInfo
    {
        public TableInfoType Type { get; set; }
    }
}
