using FootprintViewer.ViewModels.Settings;

namespace FootprintViewer.Designer;

public class DesignTimeTableInfoViewModel : TableInfoViewModel
{
    public DesignTimeTableInfoViewModel()
    {
        Type = TableInfoType.Footprint;
        Fields = GetFields(TableInfoType.Footprint);
    }
}
