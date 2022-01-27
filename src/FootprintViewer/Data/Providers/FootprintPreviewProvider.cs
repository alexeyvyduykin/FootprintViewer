using FootprintViewer.Data.Sources;
using FootprintViewer.ViewModels;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class FootprintPreviewProvider
    {
        private readonly List<IFootprintPreviewDataSource> _sources;

        public FootprintPreviewProvider()
        {
            _sources = new List<IFootprintPreviewDataSource>();
        }

        public void AddSource(IFootprintPreviewDataSource source)
        {
            _sources.Add(source);
        }

        public IEnumerable<FootprintPreview> GetFootprintPreviews()
        {
            var list = new List<FootprintPreview>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetFootprintPreviews());
            }

            return list;
        }

        public IEnumerable<IFootprintPreviewDataSource> Sources => _sources;
    }
}
