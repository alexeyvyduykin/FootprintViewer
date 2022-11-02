using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System.Threading.Tasks;

namespace FootprintViewer.Designer
{
    public class DesignTimeCustomToolBar : CustomToolBar
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeCustomToolBar() : base(_designTimeData)
        {
          //  var provider = _designTimeData.GetExistingService<IProvider<MapResource>>();

          //  var maps = Task.Run(async () => await provider.GetNativeValuesAsync(null)).Result;

          //  MapBackgroundList.Update(maps);
        }
    }
}
