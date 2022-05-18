using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System.Linq;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewer : GroundTargetViewer
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeGroundTargetViewer() : base(_designTimeData)
        {
            var provider = _designTimeData.GetExistingService<IProvider<GroundTargetInfo>>();

            var targets = provider.GetValuesAsync().Result;

            var list = new GroundTargetViewerList(provider);

            MainContent = list;

            list.Update(targets.Select(s => s.Name!).ToArray());
        }
    }
}
