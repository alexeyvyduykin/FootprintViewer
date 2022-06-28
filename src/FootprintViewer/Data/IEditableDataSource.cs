using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IEditableDataSource<TNative> : IDataSource<TNative>
    {
        Task AddAsync(TNative value);

        Task RemoveAsync(TNative value);

        Task EditAsync(string key, TNative value);
    }
}
