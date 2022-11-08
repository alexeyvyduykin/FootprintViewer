using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.Settings;
using Splat;
using System.Collections.Generic;

namespace FootprintViewer.Designer;

public class DesignTimeSourceContainerViewModel : SourceContainerViewModel
{
    private static readonly DesignTimeData _designTimeData = new();
    private static readonly IDataManager _dataManager = _designTimeData.GetService<IDataManager>()!;

    public DesignTimeSourceContainerViewModel() : base(DbKeys.Footprints.ToString(), _designTimeData)
    {
        var source = _dataManager.GetSources(DbKeys.Footprints.ToString())[0];

        Header = DbKeys.Footprints.ToString();

        Sources = new List<ISourceViewModel>()
        {
            new SourceViewModel(source) { Name = "Source1" },
            new SourceViewModel(source) { Name = "Source2" },
            new SourceViewModel(source) { Name = "Source3" },
        };
    }
}
