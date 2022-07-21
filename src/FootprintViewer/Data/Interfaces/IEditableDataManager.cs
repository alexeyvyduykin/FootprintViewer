using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IEditableDataManager<TNative> : IDataManager<TNative>
    {
        Task AddAsync(IDataSource dataSource, TNative value);

        Task RemoveAsync(IDataSource dataSource, TNative value);

        Task EditAsync(IDataSource dataSource, string key, TNative value);
    }
}
