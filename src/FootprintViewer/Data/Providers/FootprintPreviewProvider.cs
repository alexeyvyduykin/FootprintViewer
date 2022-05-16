using FootprintViewer.Data.Sources;
using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class FootprintPreviewProvider : BaseProvider<IFootprintPreviewDataSource>, IProvider<FootprintPreview>
    {
        public async Task<List<FootprintPreview>> GetValuesAsync(IFilter<FootprintPreview>? filter)
        {
            return await Task.Run(() =>
            {
                var list = new List<FootprintPreview>();

                foreach (var source in Sources)
                {
                    list.AddRange(source.GetFootprintPreviews(filter));
                }

                return list;
            });
        }
    }
}
