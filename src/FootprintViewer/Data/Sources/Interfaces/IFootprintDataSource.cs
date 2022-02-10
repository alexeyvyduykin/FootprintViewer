using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public interface IFootprintDataSource
    {
        Task<List<Footprint>> GetFootprintsAsync();
    }
}
