using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers.Providers;
using FootprintViewer.ViewModels.SidePanel.Tabs;
using Mapsui.Nts;
using Splat;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Designer;

public class DesignTimeGroundTargetTab : GroundTargetTabViewModel
{
    private static readonly IReadonlyDependencyResolver _dependencyResolver = new DesignTimeData();

    public DesignTimeGroundTargetTab() : base(_dependencyResolver)
    {
        var provider = _dependencyResolver.GetExistingService<GroundTargetProvider>();
        var dataManager = _dependencyResolver.GetExistingService<IDataManager>();

        var res = Task.Run(async () => await dataManager.GetDataAsync<GroundTarget>(DbKeys.GroundTargets.ToString())).Result;

        var features = res.Select(s => new GeometryFeature() { ["Name"] = s.Name });

        provider.ActiveFeaturesChanged.Execute(features).Subscribe();

        Filter.IsActive = true;

        IsActive = true;
    }
}
