using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System.Linq;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewerList : GroundTargetViewerList
    {
        private static readonly DesignTimeData _designTimeData = new DesignTimeData();

        public DesignTimeGroundTargetViewerList() : base(_designTimeData.GetExistingService<GroundTargetProvider>())
        {
            var provider = _designTimeData.GetExistingService<GroundTargetProvider>();

            var targets = provider.GetValuesAsync(null).Result;

            Update(targets.Select(s => s.Name!).ToArray());
        }
    }
}
