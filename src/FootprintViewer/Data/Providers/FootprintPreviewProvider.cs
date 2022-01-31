using FootprintViewer.Data.Sources;
using FootprintViewer.ViewModels;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class FootprintPreviewProvider : BaseProvider<IFootprintPreviewDataSource>
    {
        public IEnumerable<FootprintPreview> GetFootprintPreviews()
        {
            var list = new List<FootprintPreview>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetFootprintPreviews());
            }

            return list;
        }
    }
}
