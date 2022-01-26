using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace UserGeometriesDatabaseSample.Data
{
    public interface IDataSource
    {
        void Add(UserGeometry geometry);

        void Remove(UserGeometry geometry);

        Task<List<UserGeometry>> LoadUsersAsync();

        ReactiveCommand<Unit, Unit> Update { get; }
    }
}
