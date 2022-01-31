using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public interface IUserGeometryDataSource
    {
        Task AddAsync(UserGeometry geometry);

        void Remove(UserGeometry geometry);

        IEnumerable<UserGeometry> GetUserGeometries();
    }
}
