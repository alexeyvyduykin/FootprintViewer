using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System.Linq;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewerList : GroundTargetViewerList
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeGroundTargetViewerList() : base(_designTimeData.GetExistingService<IProvider<GroundTargetInfo>>())
        {
            var provider = _designTimeData.GetExistingService<IProvider<GroundTargetInfo>>();

            var targets = provider.GetValuesAsync().Result;

            Update(targets.Select(s => s.Name!).ToArray());
        }
    }
}
