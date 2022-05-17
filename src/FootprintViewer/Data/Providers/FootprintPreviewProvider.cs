using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class FootprintPreviewProvider : BaseProvider<IDataSource<FootprintPreview>>, IProvider<FootprintPreview>
    {
        public async Task<List<FootprintPreview>> GetValuesAsync(IFilter<FootprintPreview>? filter)
        {
            return await Task.Run(async () =>
            {
                var list = new List<FootprintPreview>();

                foreach (var source in Sources)
                {
                    var values = await source.GetValuesAsync(filter);
                    list.AddRange(values);
                }

                return list;
            });
        }
    }
}
