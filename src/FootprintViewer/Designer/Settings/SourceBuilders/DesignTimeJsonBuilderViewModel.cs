using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels.Settings.SourceBuilders;
using System.IO;
using System.Reflection;

namespace FootprintViewer.Designer;

public class DesignTimeJsonBuilderViewModel : JsonBuilderViewModel
{
    public DesignTimeJsonBuilderViewModel() : base(DbKeys.Footprints.ToString())
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var path = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets"));

        Directory = path;

        IsActive = true;
    }
}
