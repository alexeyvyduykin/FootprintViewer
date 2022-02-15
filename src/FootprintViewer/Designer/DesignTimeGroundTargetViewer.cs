using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewer : GroundTargetViewer
    {
        private static readonly DesignTimeData _designTimeData = new DesignTimeData();

        public DesignTimeGroundTargetViewer() : base(_designTimeData)
        {
            var provider = _designTimeData.GetExistingService<GroundTargetProvider>();

            var targets = Task.Run(async () => await provider.GetGroundTargetsAsync()).Result;
            
            var list = new GroundTargetViewerList(provider);

           // MainContent = list;

            list.Update(targets.Select(s => s.Name).ToArray());
        }
    }
}
