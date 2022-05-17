using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IEditableDataSource<T> : IDataSource<T>
    {
        Task AddAsync(T value);

        Task RemoveAsync(T value);

        Task EditAsync(string key, T value);
    }
}
