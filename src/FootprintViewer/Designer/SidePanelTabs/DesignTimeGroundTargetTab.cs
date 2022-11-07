using FootprintViewer.ViewModels.SidePanel.Tabs;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetTab : GroundTargetTabViewModel
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeGroundTargetTab() : base(_designTimeData)
        {
            IsActive = true;

            //    var provider = _designTimeData.GetExistingService<IProvider<GroundTarget>>();

            //   var arr = Task.Run(async () => await provider.GetNativeValuesAsync(null)).Result;

            //    NameFilter.FilterNames = arr.Select(s => s.Name!).ToArray();
        }
    }
}
