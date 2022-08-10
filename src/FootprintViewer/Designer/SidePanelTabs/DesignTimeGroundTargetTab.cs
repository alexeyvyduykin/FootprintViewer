using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetTab : GroundTargetTab
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeGroundTargetTab() : base(_designTimeData)
        {
            IsActive = true;

            var provider = _designTimeData.GetExistingService<IProvider<GroundTarget>>();

            var arr = Task.Run(async () => await provider.GetNativeValuesAsync(null)).Result;

            NameFilter.FilterNames = arr.Select(s => s.Name!).ToArray();
        }
    }
}
