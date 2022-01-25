using FootprintViewer.ViewModels;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public interface IUserDataSource
    {
        IEnumerable<FootprintPreview> GetFootprints();

        IList<LayerSource> WorldMapSources { get; }
    }
}
