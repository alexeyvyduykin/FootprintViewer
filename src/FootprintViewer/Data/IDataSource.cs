using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IDataSource<T>
    {
        Task<List<T>> GetValuesAsync(IFilter<T>? filter);
    }

    //public interface IEditableDataSource<T> : IDataSource<T>
    //{
    //    Task AddAsync(UserGeometry geometry);

    //    Task RemoveAsync(UserGeometry geometry);

    //    Task UpdateGeometry(string key, NetTopologySuite.Geometries.Geometry geometry);
    //}
}
