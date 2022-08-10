using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System.Threading.Tasks;

namespace FootprintViewer.Designer
{
    public class DesignTimeMapBackgroundList : MapBackgroundList
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeMapBackgroundList() : base()
        {
            var provider = _designTimeData.GetExistingService<IProvider<MapResource>>();

            var maps = Task.Run(async () => await provider.GetNativeValuesAsync(null)).Result;

            Update(maps);
        }
    }
}
