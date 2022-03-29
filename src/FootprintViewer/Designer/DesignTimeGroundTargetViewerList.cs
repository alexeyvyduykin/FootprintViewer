using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewerList : GroundTargetViewerList
    {
        private static readonly DesignTimeData _designTimeData = new DesignTimeData();

        public DesignTimeGroundTargetViewerList() : base(_designTimeData.GetExistingService<GroundTargetProvider>())
        {
            var provider = _designTimeData.GetExistingService<GroundTargetProvider>();

            var targets = Task.Run(async () => await provider.GetGroundTargetsAsync()).Result;

            Update(targets.Select(s => s.Name!).ToArray());
        }
    }
}
